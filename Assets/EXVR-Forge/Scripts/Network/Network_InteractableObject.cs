﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(InteractableHoverEvents), typeof(NetworkIdentity))]
public class Network_InteractableObject : NetworkBehaviour
{
    public bool isAttached = false;

    private NetworkInstanceId nid;

    private void Start()
    {
        nid = GetComponent<NetworkIdentity>().netId;
        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnPickup);
        GetComponent<InteractableHoverEvents>().onDetachedFromHand.AddListener(OnRelease);
    }

    public void OnPickup()
    {
        Network_PlayerController npc = GetLocalPlayerController();
        npc.CmdOnGrab(nid, npc.gameObject.GetComponent<NetworkIdentity>());
        npc.CmdOnPickup(nid);
    }

    public void OnRelease()
    {
        Network_PlayerController npc = GetLocalPlayerController();
        npc.CmdOnRelease(nid);
    }

    public Network_PlayerController GetLocalPlayerController()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");

        if (player) return player.GetComponent<Network_PlayerController>();
        else return null;
    }

    [ClientRpc]
    public void RpcOnPickup()
    {
        isAttached = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    [ClientRpc]
    public void RpcOnRelease()
    {
        isAttached = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
