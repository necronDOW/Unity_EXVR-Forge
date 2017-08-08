using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VRPlayerController : NetworkBehaviour
{
    [Command]
    public void CmdSetAuth(NetworkInstanceId objectId, NetworkIdentity player)
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
}
