using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class Hand : MonoBehaviourPunCallbacks
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public GameObject M_Pointer;
    public SteamVR_Action_Boolean m_TeleportAction;
   // public SteamVR_Action_Boolean m_ClickAction;


    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private bool m_HasPosition = false;
    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        if (photonView.IsMine)
        {
            m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
            m_Joint = GetComponent<FixedJoint>();
        }
     
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            
            //Down
            if (m_GrabAction.GetStateDown(m_Pose.inputSource))
            {
                print(m_Pose.inputSource + "Trigger Down");
                Pickup();
                
            }

            //Up
            if (m_GrabAction.GetStateUp(m_Pose.inputSource))
            {
                print(m_Pose.inputSource + "Trigger Up");
                Drop();
            }

            //Pointer
            m_HasPosition = UpdatePointer();
            M_Pointer.SetActive(m_HasPosition);

            // Teleport
            if (m_TeleportAction.GetLastStateUp(m_Pose.inputSource))
                TryTeleport();


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
            return;

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    [PunRPC]
    public void Pickup()
    {
        //Get nearest
        m_CurrentInteractable = GetNearestInteractable();

        //Null Check
        if (!m_CurrentInteractable)
            return;

        // Already held,check
        if (m_CurrentInteractable.m_ActiveHand)
            m_CurrentInteractable.m_ActiveHand.Drop();

        //Position
        m_CurrentInteractable.transform.position = transform.position;

        //Attach
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        //Set active hand
        m_CurrentInteractable.m_ActiveHand = this;

    }

    [PunRPC]
    public void Drop()
    {
        //Null check
        if (!m_CurrentInteractable)
            return;

        //Apply velocity
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

        //Detach
        m_Joint.connectedBody = null;

        //clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach (Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }

    [PunRPC]
    private void TryTeleport()
    {
        //Check for valid position, and if already teleporting
        if (!m_HasPosition || m_IsTeleporting)
            return;
        //GameObject.Find("M_Pointer").active = false;

        //Get Camera Rig, and hand position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;

        //Figure out translation
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 translateVector = M_Pointer.transform.position - groundPosition;

        //Move
        StartCoroutine(MoveRig(cameraRig, translateVector));
    }

  
    [PunRPC]
    private IEnumerator MoveRig(Transform CameraRig, Vector3 translation)
    {
        //Flag
        m_IsTeleporting = true;

        //Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);

        //Apply translation
        yield return new WaitForSeconds(m_FadeTime);
        CameraRig.position += translation;

        //Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);

        //De-flag
        m_IsTeleporting = false;


        yield return null;
    }

    [PunRPC]
    private bool UpdatePointer()
    {
        //Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //If it`s a hit
        if (Physics.Raycast(ray, out hit))
        {
            M_Pointer.transform.position = hit.point;
            return true;
        }

        //If not a hit
        return false;
    }


}
