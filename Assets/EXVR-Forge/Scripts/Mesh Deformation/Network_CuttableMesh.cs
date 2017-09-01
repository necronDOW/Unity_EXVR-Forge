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
    }

    public void test()
    {
        Debug.Log("test");
    }
}
