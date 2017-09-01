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
    public void CmdOnCut(NetworkInstanceId objectId, Vector3 one, Vector3 two, float f)
    {
        RpcOnCut();
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        GameObject[] halves = MeshCutter.MeshCut.Cut(iObject, one, two, f);

        for (int i = 0; i < halves.Length; i++) {
            Destroy(halves[i], 2.0f);
            NetworkServer.Spawn(halves[i]);
        }
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
