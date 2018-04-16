using UnityEngine;
using UnityEngine.Networking;


public class HostGame : MonoBehaviour
{

    [SerializeField]
    private uint roomSize = 6;

    private string roomName;

    private NetworkManager networkManager;

    private void Start()
    {//putting the networkManager in cache to be more efficient
        networkManager = NetworkManager.singleton;
        //Enable matchMaker if it hasn't been done already
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name)
    {
        Debug.Log("room name changed to " + _name);
        roomName = _name;
    }

    public void CreateRoom()
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " with room for " + roomSize + " players");
            //setup of the mathmaker with default parameters, only changing room name and size here
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
        }
    }
}
