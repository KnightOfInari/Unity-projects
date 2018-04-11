using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerControler))]
[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    //Sets the good layer to non local players and disable control to them from the local client
    //Remove the Scene camera, only letting each players camera
    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
       
            // Disable player graphics for local player
            setLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create PlayerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            // Configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab");
            ui.SetController(GetComponent<PlayerControler>());
        }

        GetComponent<PlayerManager>().Setup();
    }

    void setLayerRecursively(GameObject obj, int newLayer)
    {
        //change the layer of the object
        obj.layer = newLayer;

        // looping through each child the gameObject has
        foreach (Transform child in obj.transform)
        {
            setLayerRecursively(child.gameObject, newLayer);
        }
    }

    //Sets multiplayer infos up when client beggins
    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    //Assigns the remote layer to current this gameObject
    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    //Disable components on this game object (can be scripts or anything given to the array in the inspector
    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    // Behaviour adopted by the game object when it is disabled.
    private void OnDisable()
    {
        Destroy(playerUIInstance);

        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

}
