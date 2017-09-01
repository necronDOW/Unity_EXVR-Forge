using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    CuttableMesh meshCut;

    private void Start()
    {
        meshCut = GetComponent<CuttableMesh>();
    }

    [Command]
    public void CmdOnCut(NetworkInstanceId objectId, Vector3 v1, Vector3 v2, float f)
    {
        RpcOnCut();

        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        GameObject[] halves = MeshCutter.MeshCut.Cut(iObject, v1, v2, f);
        Debug.Log(iObject.name);
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
