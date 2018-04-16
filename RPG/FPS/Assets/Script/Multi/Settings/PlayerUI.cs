using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerControler controler;

    void Start()
    {
        PauseMenu.IsOn = false;
    }

    public void SetController(PlayerControler _controler)
    {
        if (_controler == null)
            Debug.LogError("playerController is null");
        controler = _controler;
    }

    void Update()
    {
        SetFuelAmount(controler.GetThrusterFuelAmount());

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }
}
