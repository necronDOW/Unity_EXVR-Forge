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
            GameObject serverInstance = GameObject.Instantiate(halves[i]);
            Destroy(serverInstance, 1.0f);
            NetworkServer.Spawn(serverInstance);
        }
    }

    [ClientRpc]
    public void RpcOnCut()
    {
        GetComponent<CuttableMesh>().DisableCut();
    }
}
