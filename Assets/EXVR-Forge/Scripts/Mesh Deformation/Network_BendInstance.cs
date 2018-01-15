using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_BendInstance : NetworkBehaviour
{
    public void UpdateNetworkDeform(BendInstance bendInstance)
    {
        Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
        npc.CmdOnBend(netId, bendInstance.curvature, bendInstance.length, bendInstance.amount, bendInstance.direction);
    }

    public void NetworkUpdateMeshCollider()
    {
        Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
        npc.CmdUpdateBendColliders(netId);
    }
}
