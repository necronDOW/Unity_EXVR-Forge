using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class VRNetworkRepresentation : MonoBehaviour
{
    public string VRParentName = "";
    public float headOffset = 0.15f;

    private Transform vrHead, gfxHead;
    private Transform[] vrHands = new Transform[2];
    private Transform[] gfxHands = new Transform[2];

    private NetworkIdentity identity;

    private void Start()
    {
        identity = GetComponent<NetworkIdentity>();

        gfxHead = transform.Find("Head");
        if (gfxHead)
            gfxHead.localPosition = new Vector3(headOffset, 0.0f, 0.0f);

        if (!identity.isLocalPlayer)
        {
            transform.Find("Head").gameObject.layer = 0;
            enabled = false;
        }

        FindVRObjects();
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
}
