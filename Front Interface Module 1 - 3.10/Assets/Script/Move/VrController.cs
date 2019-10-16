using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VrController : MonoBehaviour
{
    public float m_sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;

    //controller 
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_charactercontroller = null;
    private Transform m_cameraRig = null;
    private Transform m_Head = null;

    private void Awake()
    {
        m_charactercontroller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //controller
        m_cameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHead();
        CalculateMovement();
        HandleHeight();
    }

    private void HandleHead()
    {
        //store the current position and rotation of the head 
        Vector3 oldPosition = m_cameraRig.position;
        Quaternion oldRotation = m_cameraRig.rotation;

        //taking rotation value
        transform.eulerAngles = new Vector3(0.0f, m_Head.rotation.eulerAngles.y, 0.0f);

        //restore value 
        m_cameraRig.position = oldPosition;
    }

    private void CalculateMovement()
    {

    }

    private void HandleHeight()
    {

    }
}
