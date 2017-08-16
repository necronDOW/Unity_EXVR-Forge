using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR;

public class BendInteract : MonoBehaviour
{
    public Object bendPrefab;
    public GameObject bendingTool;
    
    private SteamVR_Controller.Device device;
    private GameObject topMostParent;
    private bool bending = false;

    void Start()
    {
        if (GetComponent<SteamVR_TrackedObject>())
            device = SteamVR_Controller.Input((int)GetComponent<SteamVR_TrackedObject>().index);
    }

    void Update()
    {
        if (bending && device.GetPressUp(EVRButtonId.k_EButton_Grip))
        {
            topMostParent.GetComponentInChildren<Bend>().SetTarget(null);
            bending = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Maluable")
        {
            topMostParent = other.attachedRigidbody.gameObject;

            if (!bending && device.GetPress(EVRButtonId.k_EButton_Grip))
            {
                if (topMostParent.GetComponentInChildren<Bend>())
                {
                    topMostParent.GetComponentInChildren<Bend>().SetTarget(gameObject);
                    bending = true;
                }
            }
        }
    }
}
