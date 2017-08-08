using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(InteractableHoverEvents))]
public class AuthHandler : MonoBehaviour
{
    private void Start()
    {
        GetComponent<InteractableHoverEvents>().onAttachedToHand.AddListener(OnGrab);
    }

    public void OnGrab()
    {
        GameObject player = GameObject.FindGameObjectWithTag("VRLocalPlayer");
        NetworkIdentity playerID = player.GetComponent<NetworkIdentity>();
        player.GetComponent<VRPlayerController>().CmdSetAuth(GetComponent<NetworkIdentity>().netId, playerID);
    }
}
