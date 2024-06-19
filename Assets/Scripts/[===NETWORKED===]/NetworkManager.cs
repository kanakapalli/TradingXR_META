using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public event Action ConnectServer;
    public event Action JoinLobby;
    public event Action LeftLobby;
    public event Action CreateRoom;
    public event Action CreateRoomFailed;
    public event Action JoinRoom;
    public event Action JoinRandomRoom;
    public event Action JoinRoomFailed;
    public event Action JoinRandomRoomFailed;
    public event Action LeftRoom;
    public event Action<Photon.Realtime.Player> PlayerEntered;
    public event Action<Photon.Realtime.Player> PlayerExited;
    public event Action GameStarted;
    public event Action GameEnded;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        string _message = "NetworkManager Started";

        /*NetworkCallbacks.DebugLog(_message,
            "cyan",
            NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLog(_message,
            "cyan",
            NetworkCallbacks.DebugFont(FontStyle.italic));*/
        NetworkCallbacks.DebugLogRich(_message,
            "cyan",
            NetworkCallbacks.DebugFont(FontStyle.bold),
            NetworkCallbacks.DebugFont(FontStyle.italic));
    }

    public override void OnConnectedToMaster()
    {
        NetworkCallbacks.DebugLog("Connected to master server successfully...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
        ConnectServer?.Invoke();
    }

    public override void OnConnected()
    {
        NetworkCallbacks.DebugLog("Connected to server successfully...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
    }

    public override void OnCreatedRoom()
    {
        NetworkCallbacks.DebugLog("Created room successfully...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
        CreateRoom?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        NetworkCallbacks.DebugLog("Joined room successfully...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
        JoinRoom?.Invoke();
        Started();
    }

    public override void OnLeftRoom()
    {
        NetworkCallbacks.DebugLog("LEFT ROOM...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        LeftRoom?.Invoke();
        Ended();
    }

    public override void OnJoinedLobby()
    {
        NetworkCallbacks.DebugLog("Joined lobby successfully...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
        JoinLobby?.Invoke();
    }

    public override void OnLeftLobby()
    {
        NetworkCallbacks.DebugLog("LEFT LOBBY...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        LeftLobby?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        NetworkCallbacks.DebugLog("CREATE ROOM FAILED...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLog(string.Concat(returnCode, " :: ", message), "red", NetworkCallbacks.DebugFont(FontStyle.italic));
        CreateRoomFailed?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        NetworkCallbacks.DebugLog("JOIN ROOM FAILED...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLog(string.Concat(returnCode, " :: ", message), "red", NetworkCallbacks.DebugFont(FontStyle.italic));
        JoinRoomFailed?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        NetworkCallbacks.DebugLog("JOIN RANDOM ROOM FAILED...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLog(string.Concat(returnCode, " :: ", message), "red", NetworkCallbacks.DebugFont(FontStyle.italic));
        JoinRandomRoomFailed?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        NetworkCallbacks.DebugLog("Player Entered Room...", "green", NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLogRich(newPlayer.NickName, 
            "cyan", 
            NetworkCallbacks.DebugFont(FontStyle.italic),
            NetworkCallbacks.DebugFont(FontStyle.bold));

        PlayerEntered?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        NetworkCallbacks.DebugLog("PLAYER LEFT ROOM...", "red", NetworkCallbacks.DebugFont(FontStyle.bold));
        NetworkCallbacks.DebugLogRich(otherPlayer.NickName, 
            "cyan", 
            NetworkCallbacks.DebugFont(FontStyle.italic), 
            NetworkCallbacks.DebugFont(FontStyle.bold));

        PlayerExited?.Invoke(otherPlayer);
    }

    private void Started()
    {
        GameStarted?.Invoke();
    }

    private void Ended()
    {
        GameEnded?.Invoke();
    }
}
