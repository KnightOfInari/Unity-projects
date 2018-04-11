using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerControler controler;

    public void SetController(PlayerControler _controler)
    {
        if (_controler == null)
            Debug.LogError("playerController is null");
        controler = _controler;
    }

    void Update()
    {
        SetFuelAmount(controler.GetThrusterFuelAmount());
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }
}
