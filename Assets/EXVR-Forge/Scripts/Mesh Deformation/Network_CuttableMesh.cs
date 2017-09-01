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
    public void CmdOnCut(NetworkInstanceId objectId, Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        RpcOnCut();

        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        GameObject[] halves = MeshCutter.MeshCut.Cut(iObject, anchorPoint, normalDirection, distanceLimit);
        NetworkServer.Spawn(halves[1]);
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
