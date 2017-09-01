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
    public void CmdOnCut(Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        RpcOnCut(anchorPoint, normalDirection, distanceLimit);
    }

    [ClientRpc]
    public void RpcOnCut(Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        MeshCutter.MeshCut.Cut(gameObject, anchorPoint, normalDirection, distanceLimit);
        GetComponent<CuttableMesh>().DisableCut();
    }
}
