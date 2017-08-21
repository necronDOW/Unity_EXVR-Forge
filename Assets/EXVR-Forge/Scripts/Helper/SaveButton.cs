using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class SaveButton : MonoBehaviour
{
    public OilScript oilTarget;
    private SteamVR_Controller.Device device;

    private void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown() || ((hand.controller != null) && hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)))
        {
            if (oilTarget && oilTarget.exportTarget != null)
            {
                STL.ExportBinary(oilTarget.exportTarget, Application.dataPath + "/Exports/", "object_export");
                Debug.Log("Done");
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }
        }
    }
    //    void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log(other.name);
    //    if (other.attachedRigidbody.tag == "VrController")
    //    {
    //        SteamVR_TrackedObject obj = other.attachedRigidbody.GetComponent<SteamVR_TrackedObject>();

    //        if (obj)
    //            device = SteamVR_Controller.Input((int)obj.index);
    //    }
    //}

    //void OnTriggerStay(Collider other)
    //{
    //    if (other.attachedRigidbody.tag == "VrController" && device != null)
    //    {
    //        SteamVR_TrackedObject obj = other.attachedRigidbody.GetComponent<SteamVR_TrackedObject>();

    //        if (obj && (int)obj.index == device.index && device.GetHairTriggerDown())
    //        {
    //            if (oilTarget && oilTarget.exportTarget != null)
    //            {
    //                STL.ExportBinary(oilTarget.exportTarget, Application.dataPath + "/Exports/", "object_export");
    //                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    //            }
    //        }
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.attachedRigidbody.tag == "VrController")
    //    {
    //        SteamVR_TrackedObject obj = other.attachedRigidbody.GetComponent<SteamVR_TrackedObject>();

    //        if (obj && (int)obj.index == device.index)
    //            device = null;
    //    }
    //}
}
