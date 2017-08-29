using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    public class MeshStateHandler : MonoBehaviour
    {
        private void OnAttachedToHand(Hand hand)
        {
            Debug.Log("attached");
        }

        private void OnDetachedFromHand(Hand hand)
        {
            Debug.Log("detached");
        }
    }
}