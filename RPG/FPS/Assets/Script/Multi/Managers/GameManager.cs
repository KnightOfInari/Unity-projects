using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    //creates a unique instance of the game manager in awake
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in the scene.");
        }
        else
            instance = this;
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }

    #region Player tracking

    private const string PLAYER_ID_PREFIX = "Player";
    //Static Dictionary with player names and reference to the playerManager class
    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    //sets playerID, adds infos to the dictionnary, sets the name of the player to playerID
    public static void RegisterPlayer(string _netID, PlayerManager _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    //get player out of the Dictionnary
    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }
    // returns a player info depending on the playeriD given
    public static PlayerManager GetPlayer(string _playerID)
    {
        return players[_playerID];
    }


    ////////////////////////////
    //Shows player ID on the screen, used for testing and differentation of players at the moment

    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(200,200,200,500));

    //    GUILayout.BeginVertical();

    //    foreach(string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + " - " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();

    //    GUILayout.EndArea();
    //}
    #endregion
}
