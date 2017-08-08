using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(InteractableHoverEvents))]
public class Network_InteractableObject : NetworkBehaviour
{
    private void Start()
    {
        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnGrab);
    }

    public void OnGrab()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        NetworkInstanceId objectID = GetComponent<NetworkIdentity>().netId;

        Network_PlayerController npc = player.GetComponent<Network_PlayerController>();
        npc.CmdOnGrab(objectID, playerID);
        npc.RpcDetachObject(objectID);
    }
}
