using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_BendTool : NetworkBehaviour
{
    private BendTool bendTool;
    private GameObject lastBendInstance; 
    public bool hasSpawned { get { return lastBendInstance != null; } }

    private void Start()
    {
        bendTool = GetComponentInChildren<BendTool>();
    }

    [Command]
    public void CmdOnAttachToAnvil()
    {
        if (bendTool) {
            lastBendInstance = Instantiate(bendTool.bendPrefab, bendTool.attachedRod.transform);
            lastBendInstance.GetComponent<BendInstance>().target = bendTool.attachedRod;
            NetworkServer.Spawn(lastBendInstance);
        }
    }

    [Command]
    public void CmdDestroyAllBendInstances()
    {
        if (lastBendInstance)
            NetworkServer.Destroy(lastBendInstance);
    }
}
