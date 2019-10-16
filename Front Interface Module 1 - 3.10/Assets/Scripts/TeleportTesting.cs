using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using Valve.VR;

public class TeleportTesting : MonoBehaviour
{
    public Transform cameraRig;

    void Testing()
    {
        //head position
        Vector3 headposition = SteamVR_Render.Top().head.position;

        //poistion = headposition 
        Vector3 position = new Vector3(headposition.x, cameraRig.position.y, headposition.z);

        Debug.Log("Posiiton : " + position);
    }
}
