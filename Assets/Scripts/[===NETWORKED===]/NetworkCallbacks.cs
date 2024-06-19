using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkCallbacks
{
    public static void DebugLog(string message, string color, string style)
    {
        Debug.Log(string.Concat("<color=", color, ">", "<", style, ">", message, "</", style, ">", "</color>"));
    }

    public static void DebugLogRich(string message, string color, string style_one, string style_two)
    {
        Debug.Log(string.Concat("<color=", color, ">", "<", style_one, ">", "<", style_two, ">", message, "</", style_two, ">", "</", style_one, ">", "</color>"));
    }

    public static string DebugFont(FontStyle fontStyle)
    {
        string _res = fontStyle == FontStyle.bold ? "b" : "i";
        return _res;
    }

    public static void ConnectServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void CreateRoom(string _Room_Name, int _Max_Players)
    {
        PhotonNetwork.CreateRoom(_Room_Name, new RoomOptions { MaxPlayers = _Max_Players > 2 ? 2 : _Max_Players <= 0 ? 2 : _Max_Players }, null);
    }

    public static void JoinRoom(string _Room_Name)
    {
        PhotonNetwork.JoinRoom(_Room_Name);
    }

    public static void JoinOrCreateRoom(string _Room_Name, int _Max_Players)
    {
        PhotonNetwork.JoinOrCreateRoom(_Room_Name, new RoomOptions { MaxPlayers = _Max_Players }, null);
    }

    public static void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public static void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public static void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public static void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}

public enum FontStyle
{
    bold,
    italic
}
