using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class MeshStateHandler : MonoBehaviour
{
    private Rigidbody rigidBody;
    private MeshCollider mCollider;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        mCollider = GetComponent<MeshCollider>();
    }

    private void OnAttachToHand(Hand hand)
    {
        ChangeState(true);
    }

    private void OnDetachFromHand(Hand hand)
    {
        ChangeState(false);
    }

    public void ChangeState(bool isKinematic)
    {
        rigidBody.isKinematic = isKinematic;
        mCollider.convex = !isKinematic;
    }
}