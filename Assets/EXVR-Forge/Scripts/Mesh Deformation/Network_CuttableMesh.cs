using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnCut(Vector3 v1, Vector3 v2, float f)
    {
        RpcOnCut();
        
        GameObject[] halves = MeshCutter.MeshCut.Cut(gameObject, v1, v2, GetComponent<CuttableMesh>().rodPrefab, f);

        if (halves != null) {
            for (int i = 0; i < halves.Length; i++) {
                NetworkServer.Spawn(halves[i]);
            }
        }
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
