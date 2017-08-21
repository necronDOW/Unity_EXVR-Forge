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
                STL.ExportBinary(oilTarget.exportTarget.GetComponent<MeshFilter>(), Application.dataPath + "/Exports/", "object_" + System.DateTime.Now);
                Destroy(oilTarget.exportTarget);
                Debug.Log("Object Exported");
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
            }
        }
    }
}
