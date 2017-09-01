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

    [ClientRpc]
    public void RpcOnCut(NetworkInstanceId objectId, Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        MeshCutter.MeshCut.Cut(iObject, anchorPoint, normalDirection, distanceLimit);
        iObject.GetComponent<CuttableMesh>().DisableCut();
    }
}
