using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnCut()
    {
        Debug.Log("server");
        RpcOnCut();
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        Debug.Log("client");
    }
}
