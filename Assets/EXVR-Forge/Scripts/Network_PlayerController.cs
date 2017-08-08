using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_PlayerController : NetworkBehaviour
{
    private Network_PlayerRepresentation npr;

    private void Start()
    {
        npr = GetComponent<Network_PlayerRepresentation>();
    }

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
            {
                networkId.RemoveClientAuthority(otherOwner);
                RpcDetachObject(objectId);
            }
            networkId.AssignClientAuthority(player.connectionToClient);
        }
    }

    [ClientRpc]
    public void RpcDetachObject(NetworkInstanceId objectId)
    {
        GameObject iObject = NetworkServer.FindLocalObject(objectId);
        
        for (int i = 0; i < npr.vrHands.Length; i++)
        {
            Hand hand = npr.vrHands[i].GetComponent<Hand>();

            if (iObject == hand.currentAttachedObject)
                hand.DetachObject(hand.currentAttachedObject);
        }
    }
}
