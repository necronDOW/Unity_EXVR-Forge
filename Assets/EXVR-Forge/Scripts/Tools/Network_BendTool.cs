using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BendTool : NetworkBehaviour
{
    public BendTool bendTool { get; private set; }
    public BendInstance bendInstance = null;

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
        bendInstance = bendInstanceLocal.GetComponent<BendInstance>();

        bendInstanceLocal.transform.parent = bendTool.attachedRod.transform;
        bendInstanceLocal.transform.position = bendTool.transform.position;
        bendInstanceLocal.transform.rotation = bendTool.attachedRod.transform.rotation;
        bendInstance.Initialize();
    }

    [ClientRpc]
    public void RpcRecalculateBounds()
    {
        bendTool.attachedRod.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
    }
}
