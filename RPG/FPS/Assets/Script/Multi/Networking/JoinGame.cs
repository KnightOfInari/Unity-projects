﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{

    private NetworkManager networkManager;

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    //Sets the network manager to the single existing instance. Refreshes room list to see if there are already existing servers
    private void Start()
    {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        RefreshRoomList();
    }

    //Refreshes servers room list
    public void RefreshRoomList()
    {
        ClearRoomList(); //clears list before refreshing it
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading..."; 
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
        //chekcs if list is not null and if it was gotten
        if (!success ||matchList == null)
        {
            status.text = "unable to get room list.";
            return;
        }
        //adds found matches to list room
        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItemGO = Instantiate(roomListItemPrefab);
            _roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGO.GetComponent<RoomListItem>();
            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            ////////////////////////////////

            roomList.Add(_roomListItemGO);
        }
        if (roomList.Count == 0) // if there is no room
        {
            status.text = "No rooms found at the moment";
        }

    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        ClearRoomList();
        status.text = "Joining...";
    }
}
