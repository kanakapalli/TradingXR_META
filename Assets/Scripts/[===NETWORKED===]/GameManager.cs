using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager m_NetworkManager;
    [SerializeField] GameObject m_Player_Prefab;
    [SerializeField] Transform[] m_Player_Instantiate_LOC;

    public event Action GameOver;

    private void OnEnable()
    {
        Init();
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Init()
    {
        NetworkCallbacks.ConnectServer();
        m_NetworkManager = FindObjectOfType<NetworkManager>();
    }

    private void Subscribe()
    {
        m_NetworkManager.ConnectServer += JoinLobby;
        m_NetworkManager.JoinLobby += JoinOrCreateRoom;
        m_NetworkManager.JoinRoom += InstantiatePlayer;
        m_NetworkManager.PlayerEntered += DisableCameras;
    }

    private void UnSubscribe()
    {
        m_NetworkManager.ConnectServer -= JoinLobby;
        m_NetworkManager.JoinLobby -= JoinOrCreateRoom;
        m_NetworkManager.JoinRoom -= InstantiatePlayer;
        m_NetworkManager.PlayerEntered -= DisableCameras;
    }

    private void JoinLobby()
    {
        NetworkCallbacks.JoinLobby();
    }

    private void JoinOrCreateRoom()
    {
        NetworkCallbacks.JoinOrCreateRoom("My_Room", 2);
    }

    private void InstantiatePlayer()
    {
        NetworkCallbacks.DebugLogRich("INSTANTIATED PLAYER", 
            "blue", 
            NetworkCallbacks.DebugFont(FontStyle.bold), 
            NetworkCallbacks.DebugFont(FontStyle.italic));

        PhotonNetwork.Instantiate(m_Player_Prefab.name, m_Player_Instantiate_LOC[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
    }

    public void ChangeNickName(TMP_InputField m_NickName)
    {
        PhotonNetwork.NickName = m_NickName.text;
        GetCurrentPlayer().GetComponent<Player>().PlayerUpdate();
    }

    private void DisableCameras(Photon.Realtime.Player _Player)
    {
        foreach (GameObject _pl in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!_pl.GetComponent<PhotonView>().IsMine)
            {
                _pl.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    internal Transform GetCurrentPlayer()
    {
        foreach (GameObject _pl in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (_pl.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log(_pl.name);
                return _pl.transform;
            }
        }
        return null;
    }

    internal void GameOverActivity()
    {
        GameOver?.Invoke();
    }
}
