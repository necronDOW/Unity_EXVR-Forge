using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_PlayerController : NetworkBehaviour
{
    Network_PlayerRepresentation npr;

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
                networkId.RemoveClientAuthority(otherOwner);
            networkId.AssignClientAuthority(player.connectionToClient);
        }
    }

    [ClientRpc]
    public void RpcDetachObject(NetworkInstanceId objectId)
    {
        GameObject objectToDetach = NetworkServer.FindLocalObject(objectId);

        if (npr)
        {
            for (int i = 0; i < npr.vrHands.Length; i++)
            {
                Hand hand = npr.vrHands[0].GetComponent<Hand>();
                hand.DetachObject(objectToDetach);
            }
        }
    }
}
