using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using Photon.Realtime;

public class Teleport : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject Pointer;
    public SteamVR_Action_Boolean TeleportAction;

    private SteamVR_Behaviour_Pose Pose = null;
    private bool HasPosition = false;

    private bool IsTeleporting = false;
    private float FadeTime = 0.0f;

    PhotonView PV;

     public Vector3 latestPos;
    public Quaternion latestRot;

    //MyPlayer myPlayer;


    private void Awake()
    {
        Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }
    // Update is called once per frame
    void Update()
    {
        //Pointer
        HasPosition = UpdatePointer();
        Pointer.SetActive(HasPosition);

        //Teleport
        if (TeleportAction.GetLastStateUp(Pose.inputSource))
            TryTeleport();
    }


    /// ////////////////////////////////////////
    
    [PunRPC]
    private void TryTeleport()
    {

        PV = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {

            // check valid position and if already teleport 
            if (!HasPosition || IsTeleporting)
                return;

            //get camera rig and head position 
            Transform cameraRig = SteamVR_Render.Top().origin;
            Vector3 headPosition = SteamVR_Render.Top().head.position;

            //latestPos = headPosition;

            // figured out translation       
            Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
            Vector3 translateVector = Pointer.transform.position - groundPosition;

            //latestPos = groundPosition;
            Debug.Log("Position : " + groundPosition);
            //Debug.Log("Position : " + translateVector);

            // move
            // StartCoroutine(MoveRig((cameraRig, translateVector)));
            StartCoroutine(MoveRig(cameraRig, translateVector));

        }
    }

    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        //Flag 
        IsTeleporting = true;

        //Fade to black 
        SteamVR_Fade.Start(Color.black, FadeTime, true);

        // apply translation
        yield return new WaitForSeconds(FadeTime);
        cameraRig.position += translation;


        //fade to clear 
        SteamVR_Fade.Start(Color.clear, FadeTime, true);

        //de flag 
        IsTeleporting = false;
    }

    private bool UpdatePointer()
    {
        //Ray from controller 
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //if its a hit 
        if (Physics.Raycast(ray, out hit))
        {
            Pointer.transform.position = hit.point;
            return true;
        }

        //if not hit 
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //this is our player : sent data to others 
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            // smoothMove = (Vector3) stream.ReceiveNext();

            // this is network player 
            //receive data 
            /*latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();*/

            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
