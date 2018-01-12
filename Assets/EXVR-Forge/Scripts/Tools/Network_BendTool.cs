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
        if (bendTool && !initializedInstance && isServer) {
            GameObject bendInstance = Instantiate(bendTool.bendPrefab);
            NetworkServer.Spawn(bendInstance);

            initializedInstance = bendInstance.GetComponent<BendInstance>();

            RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId, 
                bendTool.attachedRod.GetComponent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId, NetworkInstanceId rodInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        GameObject rodInstanceLocal = ClientScene.FindLocalObject(rodInstanceId);
        bendInstanceLocal.transform.parent = rodInstanceLocal.transform;

        BendInstance bendInstance = bendInstanceLocal.GetComponent<BendInstance>();
        bendInstance.target = bendTool.attachedRod;
        bendInstance.Initialize();

        Debug.Log("initialized bend instance");
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (initializedInstance) {
            NetworkServer.Destroy(initializedInstance.gameObject);
        }
    }
}
