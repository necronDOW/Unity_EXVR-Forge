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

    private void Update()
    {
        if (bendTool.attachedRod) {
            RodGripScript rgs = bendTool.attachedRod.GetComponent<RodGripScript>();
            if (rgs.isGripped) {
                bendInstance.rodGripScriptReference = rgs;

                Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
                npc.CmdBendInstanceLookAtGrip(bendInstance.GetComponentInParent<NetworkIdentity>().netId, rgs.target.position);
            }
            else if (bendInstance)
                bendInstance.rodGripScriptReference = null;
        }
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
        bendInstance = bendInstanceLocal.GetComponentInChildren<BendInstance>();

        bendInstanceLocal.transform.parent = bendTool.attachedRod.transform;
        bendInstanceLocal.transform.position = bendTool.transform.position;
        bendInstanceLocal.transform.rotation = bendTool.attachedRod.transform.rotation;
        bendInstance.Initialize();
    }

    [ClientRpc]
    public void RpcDestroyAllBendInstances()
    {
        bendInstance = null;
    }
}
