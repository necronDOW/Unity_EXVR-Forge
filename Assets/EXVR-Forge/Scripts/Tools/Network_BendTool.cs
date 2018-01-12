using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BendTool : NetworkBehaviour
{
    private BendTool bendTool;
    public BendInstance initializedInstance;

    private void Start()
    {
        bendTool = GetComponentInChildren<BendTool>();
    }
    
    public void OnAttachToAnvil()
    {
        Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
        npc.CmdOnAttachBendTool(netId);
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = null;
        while (!bendInstanceLocal)
            bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);

        Debug.Log(bendTool);
        Debug.Log(bendTool.attachedRod);
        bendInstanceLocal.transform.parent = bendTool.attachedRod.transform;

        BendInstance bendInstance = bendInstanceLocal.GetComponent<BendInstance>();
        bendInstance.target = bendTool.attachedRod;
        bendInstance.Initialize();
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (initializedInstance) {
            NetworkServer.Destroy(initializedInstance.gameObject);
        }
    }
}
