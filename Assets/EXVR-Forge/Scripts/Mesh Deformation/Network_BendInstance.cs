using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_BendInstance : NetworkBehaviour
{
    public void UpdateNetworkDeform(BendInstance bendInstance)
    {
        Debug.Log("sending bend command");
        Network_PlayerController npc = Network_InteractableObject.GetLocalPlayerController();
        npc.CmdOnBend(GetComponent<NetworkIdentity>().netId, bendInstance.curvature, bendInstance.length, bendInstance.amount, bendInstance.direction);
    }
}
