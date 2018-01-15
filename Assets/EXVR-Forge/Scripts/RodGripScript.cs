using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class RodGripScript : MonoBehaviour
{
    public Transform target { get; private set; }
    public Vector3 grippedPoint { get; private set; }
    public bool isGripped { get { return target != null; } }
    private Coroutine lastRanCoroutine;

    private void Awake()
    {
        target = null;
        grippedPoint = LookAtScript.nullTarget;
    }

    private void OnHandHoverBegin(Hand hand) { lastRanCoroutine = StartCoroutine(e_OnHandHoverBegin(hand)); }
    private IEnumerator e_OnHandHoverBegin(Hand hand)
    {
        if (lastRanCoroutine != null)
            StopCoroutine(lastRanCoroutine);

        while (!((hand.controller != null) && hand.controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip)))
            yield return null;

        target = hand.transform;
        grippedPoint = hand.transform.position;
    }

    private void OnHandHoverEnd(Hand hand) { lastRanCoroutine = StartCoroutine(e_OnHandHoverEnd(hand)); }
    private IEnumerator e_OnHandHoverEnd(Hand hand)
    {
        if (lastRanCoroutine != null)
            StopCoroutine(lastRanCoroutine);

        while (((hand.controller != null) && hand.controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip)))
            yield return null;

        target = null;
        grippedPoint = LookAtScript.nullTarget;
    }
}
