using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(InteractableHoverEvents))]
public class Network_InteractableObject : NetworkBehaviour
{
    public bool isAttached = false;

    private void Start()
    {
        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnGrab);
    }

    public void OnGrab()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        player.GetComponent<Network_PlayerController>().CmdOnGrab(GetComponent<NetworkIdentity>().netId, playerID);
        player.GetComponent<Network_PlayerController>().CmdOnPickup(GetComponent<NetworkIdentity>().netId);
    }
}
