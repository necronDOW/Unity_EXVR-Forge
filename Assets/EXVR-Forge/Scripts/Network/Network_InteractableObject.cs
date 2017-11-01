using System.Collections;
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
        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnGrab);
        GetComponent<InteractableHoverEvents>().onDetachedFromHand.AddListener(OnRelease);
    }

    public void OnGrab()
    {
        RpcOnGrab();
    }

    public void OnRelease()
    {
        RpcOnRelease();
    }

    public Network_PlayerController GetLocalPlayerController()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");

        if (player) return player.GetComponent<Network_PlayerController>();
        else return null;
    }

    [ClientRpc]
    public void RpcOnGrab()
    {
        isAttached = true;
        GetComponent<Collider>().isTrigger = true;
    }

    [ClientRpc]
    public void RpcOnRelease()
    {
        isAttached = false;
        GetComponent<Collider>().isTrigger = false;
    }
}
