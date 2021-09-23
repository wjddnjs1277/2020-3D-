using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static string nickName;
    public string gameVersion = "1.0";

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = nickName;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    public override void OnJoinedRoom()
    {
        Spawn();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Sun", new Vector3(0, 500, 0), Quaternion.identity);
            PhotonNetwork.Instantiate("Moon", new Vector3(0, -500, 0), Quaternion.identity);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector3(0,2,0), Quaternion.identity);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Connect");
    }
}
