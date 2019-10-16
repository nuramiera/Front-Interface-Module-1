using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    
    string roomName = "My Room";
    bool ShowGUI = true;
    

    

    public void OnClick_ConnectBtn()
    {

         PhotonNetwork.ConnectUsingSettings();
        
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }



    public override void OnConnected()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.LoadLevel("Rooms");

    }
}
