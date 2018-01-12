using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TestScript : MonoBehaviour {

    private void HandHoverUpdate(Hand hand)
    {
        if (((hand.controller != null) && hand.controller.GetPress(Valve.VR.EVRButtonId.k_EButton_Grip)))
        {
            
        }
    }
}
