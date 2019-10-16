﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;


public class MyPlayer : MonoBehaviourPunCallbacks ,  IPunObservable
{
    public Camera cam;
    // liat of scripts that should only be active for the local player( PlayerController)
    public MonoBehaviour[] localScripts;
    // list of GameObjects that should onlynbe active for the local player (Camera,AudioListener)
    public GameObject[] localObjects;
    //values that will be synced over network
    Vector3 latestPos;
    Quaternion latestRot;
    private PhotonView PV;
  
   

    //public static GameObject LocalPlayerInstance;
   /* void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }*/

    
    #region MonoBehaviour CallBacks

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            photonView.RPC("Pickup", RpcTarget.AllBuffered);
            photonView.RPC("Drop", RpcTarget.AllBuffered);
            photonView.RPC("TryTeleport", RpcTarget.AllBuffered);


            // MyPlayer.LocalPlayerInstance = this.gameObject;
            //DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            //deactivate if it is not YOU.
           cam.enabled = false;
            //player is remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i =0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }

    }

 
    
    
   
    #endregion

   /* void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

    }*/


    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
          
        }
    
        else if (stream.IsReading)
        {
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            //Update remote Player (smooth this, this looks good, at the cost of some accuracy
           transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);

        }
        
    }
    #endregion


}
