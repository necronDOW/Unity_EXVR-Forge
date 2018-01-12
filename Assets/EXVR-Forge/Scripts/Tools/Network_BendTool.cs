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

    public void DestroyAllBendInstances()
    {
        Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
        npc.CmdDestroyAllBendInstances(netId);
    }

    [ClientRpc]
    public void RpcOnAttachToAnvil(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        while (!bendInstanceLocal)
            bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        
        bendInstanceLocal.transform.parent = bendTool.attachedRod.transform;

        BendInstance bendInstance = bendInstanceLocal.GetComponent<BendInstance>();
        bendInstance.target = bendTool.attachedRod;
        bendInstance.Initialize();
    }
}
