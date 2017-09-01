using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnCut(NetworkInstanceId objectId)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        iObject.GetComponent<Network_CuttableMesh>().test();
        RpcOnCut(objectId);
    }

    [ClientRpc]
    public void RpcOnCut(NetworkInstanceId objectId)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        iObject.GetComponent<Network_CuttableMesh>().test2();
    }

    public void test()
    {
        Debug.Log("server");
    }

    public void test2()
    {
        Debug.Log("client");
    }
}
