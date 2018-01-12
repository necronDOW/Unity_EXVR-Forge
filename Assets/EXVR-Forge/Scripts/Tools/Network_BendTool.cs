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

            SetupBendInstance(bendInstance, bendTool.attachedRod);
            RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        SetupBendInstance(bendInstanceLocal, bendTool.attachedRod);
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (initializedInstance) {
            NetworkServer.Destroy(initializedInstance.gameObject);
        }
    }

    private void SetupBendInstance(GameObject bendInstanceObject, GameObject parentObject)
    {
        bendInstanceObject.transform.parent = parentObject.transform;

        BendInstance bendInstance = bendInstanceObject.GetComponent<BendInstance>();
        bendInstance.target = bendTool.attachedRod;
        bendInstance.Initialize();
    }
}
