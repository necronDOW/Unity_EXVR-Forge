using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_PlayerController : NetworkBehaviour
{
    [Command]
    public void CmdOnGrab(NetworkInstanceId objectId, NetworkIdentity player)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        NetworkIdentity networkId = iObject.GetComponent<NetworkIdentity>();
        NetworkConnection otherOwner = networkId.clientAuthorityOwner;

        if (otherOwner == player.connectionToClient)
            return;
        else
        {
            if (otherOwner != null)
                networkId.RemoveClientAuthority(otherOwner);
            networkId.AssignClientAuthority(player.connectionToClient);
        }
    }

    [Command]
    public void CmdOnPickup(NetworkInstanceId objectId)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        iObject.GetComponent<Network_InteractableObject>().RpcOnPickup();
    }

    [Command]
    public void CmdOnRelease(NetworkInstanceId objectId)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        iObject.GetComponent<Network_InteractableObject>().RpcOnRelease();
    }

    [Command]
    public void CmdOnAttachBendTool(NetworkInstanceId netBendToolId)
    {
        GameObject netBendToolObj = ClientScene.FindLocalObject(netBendToolId);
        Network_BendTool netBendTool = netBendToolObj.GetComponent<Network_BendTool>();

        if (!netBendTool.bendInstance) {
            GameObject bendInstance = Instantiate(netBendTool.bendTool.bendPrefab);
            NetworkServer.Spawn(bendInstance);

            netBendTool.bendInstance = bendInstance.GetComponentInChildren<BendInstance>();
            netBendTool.RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId);
        }
    }

    [Command]
    public void CmdDestroyAllBendInstances(NetworkInstanceId netBendToolId)
    {
        Network_BendTool netBendTool = ClientScene.FindLocalObject(netBendToolId).GetComponent<Network_BendTool>();

        if (netBendTool.bendInstance) { 
            //update colliders
            netBendTool.RpcDestroyAllBendInstances();
            NetworkServer.Destroy(netBendTool.bendInstance.transform.parent.gameObject);    
        }
    }

    [Command]
    public void CmdOnBend(NetworkInstanceId bendId, float curvature, float length, float amount, bool direction, bool handDirection)
    {
        RpcOnBend(bendId, curvature, length, amount, direction, handDirection);
    }

    [ClientRpc]
    public void RpcOnBend(NetworkInstanceId bendInstanceId, float curvature, float length, float amount, bool direction, bool handDirection)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);

        if (bendInstanceLocal)
        {
            BendInstance bendInstanceScript = bendInstanceLocal.GetComponentInChildren<BendInstance>();

            bendInstanceScript.curvature = curvature;
            bendInstanceScript.length = length;
            bendInstanceScript.amount = amount;
            bendInstanceScript.direction = direction;
            bendInstanceScript.handDirection = handDirection;
            bendInstanceScript.DeformAll();
        }
    }

    [Command]
    public void CmdBendInstanceLookAtGrip(NetworkInstanceId bendInstanceId, Vector3 targetPosition)
    {
        RpcBendInstanceLookAtGrip(bendInstanceId, targetPosition);
    }

    [ClientRpc]
    public void RpcBendInstanceLookAtGrip(NetworkInstanceId bendInstanceId, Vector3 targetPosition)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        LookAtScript las = bendInstanceLocal.GetComponentInParent<LookAtScript>();
        las.target = targetPosition;
    }

    [Command]
    public void CmdUpdateBendColliders(NetworkInstanceId bendInstanceId)
    {
        RpcUpdateBendColliders(bendInstanceId);
    }

    [ClientRpc]
    public void RpcUpdateBendColliders(NetworkInstanceId bendInstanceId)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendInstanceId);
        BendInstance bi = bendInstanceLocal.GetComponentInChildren<BendInstance>();
        bi.UpdateMeshCollider();
    }
}
