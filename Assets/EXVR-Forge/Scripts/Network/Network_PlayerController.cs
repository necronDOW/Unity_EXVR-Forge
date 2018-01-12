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
    public void CmdOnBend(NetworkInstanceId bendId, float curvature, float length, float amount, bool direction)
    {
        RpcOnBend(bendId, curvature, length, amount, direction);
    }

    [Command]
    public void CmdOnAttachBendTool(NetworkInstanceId bendToolId)
    {
        BendTool bendTool = ClientScene.FindLocalObject(bendToolId).GetComponent<BendTool>();
        Network_BendTool netBendTool = bendTool.GetComponentInParent<Network_BendTool>();

        if (bendTool && !netBendTool.initializedInstance)
        {
            GameObject bendInstance = Instantiate(bendTool.bendPrefab);
            NetworkServer.Spawn(bendInstance);

            netBendTool.initializedInstance = bendInstance.GetComponent<BendInstance>();
            netBendTool.RpcOnAttachToAnvil(bendInstance.GetComponent<NetworkIdentity>().netId);
        }
    }

    [ClientRpc]
    public void RpcOnBend(NetworkInstanceId bendId, float curvature, float length, float amount, bool direction)
    {
        GameObject bendInstanceLocal = ClientScene.FindLocalObject(bendId);

        if (bendInstanceLocal)
        {
            BendInstance bendInstanceScript = bendInstanceLocal.GetComponent<BendInstance>();

            //bendInstanceScript.curvature = curvature;
            //bendInstanceScript.length = length;
            //bendInstanceScript.amount = amount;
            //bendInstanceScript.direction = direction;
        }
    }
}
