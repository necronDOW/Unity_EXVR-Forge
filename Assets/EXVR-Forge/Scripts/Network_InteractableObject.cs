using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(InteractableHoverEvents))]
public class Network_InteractableObject : NetworkBehaviour
{
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnGrab);
        GetComponent<InteractableHoverEvents>().onDetachedFromHand.AddListener(OnRelease);
    }

    public void OnGrab()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        player.GetComponent<Network_PlayerController>().CmdSetAuth(GetComponent<NetworkIdentity>().netId, playerID);
        CmdSetAttached(true);
    }

    public void OnRelease()
    {
        CmdSetAttached(false);
    }

    [Command]
    public void CmdSetAttached(bool value)
    {
        rigidBody.isKinematic = value;

        if (value)
        {
            Hand hand = GetComponent<Throwable>().attachedHand;
            hand.DetachObject(this.gameObject);
        }
    }
}
