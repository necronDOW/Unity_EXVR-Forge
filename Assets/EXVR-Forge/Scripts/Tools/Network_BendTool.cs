using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BendTool : NetworkBehaviour
{
    private BendTool bendTool;
    private BendInstance initializedInstance;

    private void Start()
    {
        bendTool = GetComponentInChildren<BendTool>();
    }

    [Command]
    public void CmdOnAttachToAnvil()
    {
        if (isServer && bendTool && !initializedInstance) {
            GameObject bendInstance = Instantiate(bendTool.bendPrefab, bendTool.attachedRod.transform);
            NetworkServer.Spawn(bendInstance);

            RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);

        initializedInstance = bendInstanceLocal.GetComponent<BendInstance>();
        initializedInstance.target = bendTool.attachedRod;
        initializedInstance.Initialize();
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (initializedInstance) {
            NetworkServer.Destroy(initializedInstance.gameObject);
            RpcDestroyAllBendInstances();
        }
    }

    [ClientRpc]
    public void RpcDestroyAllBendInstances()
    {
        initializedInstance = null;
    }
}
