using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TeleportVive))]
public class DeviceLinker : MonoBehaviour
{
    public GameObject[] devices;

    void Start()
    {
        List<SteamVR_TrackedObject> trackedObjects = new List<SteamVR_TrackedObject>();

        foreach (GameObject o in devices)
        {
            if (o.GetComponent<SteamVR_TrackedObject>())
                trackedObjects.Add(o.GetComponent<SteamVR_TrackedObject>());
        }

        GetComponent<TeleportVive>().Controllers = trackedObjects.ToArray();
    }
}
