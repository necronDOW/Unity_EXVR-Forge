using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class RodGripScript : MonoBehaviour
{
    public Transform target { get; private set; }
    public bool isGripped { get { return target != null; } }

    private void HandHoverUpdate(Hand hand) {
        if (((hand.controller != null) && hand.controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip)))
            target = hand.transform;
        else target = null;
    }
}
