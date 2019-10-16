using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RPCSync : MonoBehaviour
{

    private PhotonView PV;
    public int syncVariable;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            PV.RPC("RPC_Function", RpcTarget.AllBuffered);
        }
    }
    
    [PunRPC]
    void RPC_Function()
    {
        Debug.Log(syncVariable);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
