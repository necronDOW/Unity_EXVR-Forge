﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_PlayerRepresentation : MonoBehaviour
{
    public string VRParentName = "";

    private Transform vrHead, gfxHead;
    public Transform[] vrHands = new Transform[2];
    private Transform[] gfxHands = new Transform[2];

    private NetworkIdentity identity;

    private void Start()
    {
        identity = GetComponent<NetworkIdentity>();

        if (!identity.isLocalPlayer)
        {
            //SwitchLayer(0);
            enabled = false;
        }
        else
        {
            DisableRenderers();
            gameObject.tag = "VRLocalPlayer";
        }

        FindVRObjects();

        gfxHead = transform.Find("Head");
        gfxHands[0] = transform.Find("Hand1");
        gfxHands[1] = transform.Find("Hand2");
    }

    private void Update()
    {
        CopyTransform(vrHead, transform);
        CopyTransform(vrHands[0], gfxHands[0]);
        CopyTransform(vrHands[1], gfxHands[1]);
    }

    private void FindVRObjects()
    {
        GameObject parent = GameObject.Find(VRParentName);

        if (parent)
        {
            Hand[] hands = parent.GetComponentsInChildren<Hand>();
            for (int i = 0; i < hands.Length; i++)
                vrHands[i] = hands[i].transform;

            SteamVR_Camera head = parent.GetComponentInChildren<SteamVR_Camera>();
            if (head)
                vrHead = head.transform;
        }
    }

    private void CopyTransform(Transform a, Transform b)
    {
        if (a && b)
        {
            b.position = a.position;
            b.rotation = a.rotation;
        }
    }

    private void SwitchLayer(int layer)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChildren.Length; i++) {
            allChildren[i].gameObject.layer = layer;
        }
    }

    private void DisableRenderers()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < allRenderers.Length; i++) {
            allRenderers[i].enabled = false;
        }
    }
}
