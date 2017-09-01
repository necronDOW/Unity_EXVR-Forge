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
    public void CmdOnCut(GameObject[] halves)
    {
        RpcOnCut();

        for (int i = 0; i < halves.Length; i++) {
            GameObject.Instantiate(halves[i]);
            NetworkServer.Spawn(halves[i]);
        }
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
