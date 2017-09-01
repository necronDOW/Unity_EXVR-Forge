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
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        GameObject[] halves = MeshCutter.MeshCut.Cut(iObject, anchorPoint, normalDirection, distanceLimit);
        for (int i = 0; i < halves.Length; i++)
            NetworkServer.Spawn(halves[i]);

        RpcOnCut();
        NetworkServer.Destroy(this.gameObject);
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
