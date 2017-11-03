using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BendTool : NetworkBehaviour
{
    private BendTool bendTool;
    private GameObject lastBendInstance; 
    public bool hasSpawned { get { return lastBendInstance != null; } }

    private void Start()
    {
        bendTool = GetComponentInChildren<BendTool>();
    }

    [Command]
    public void CmdOnAttachToAnvil()
    {
        if (bendTool && !hasSpawned) {
            GameObject bendInstance = Instantiate(bendTool.bendPrefab, bendTool.attachedRod.transform);
            NetworkServer.Spawn(bendInstance);
            RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);

        lastBendInstance = bendInstanceLocal;
        BendInstance bendInstanceScript = lastBendInstance.GetComponent<BendInstance>();
        bendInstanceScript.target = bendTool.attachedRod;
        bendInstanceScript.Initialize();
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (lastBendInstance && hasSpawned) {
            RpcDestroyAllBendInstances();
            NetworkServer.Destroy(lastBendInstance);
        }
    }

    [ClientRpc]
    public void RpcDestroyAllBendInstances()
    {
        lastBendInstance = null;
    }
}
