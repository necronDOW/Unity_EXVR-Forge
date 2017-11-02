using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_DeformableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnDeform(Vector3 impactVector, Vector3 simplifiedVector)
    {
        RpcOnDeform(impactVector, simplifiedVector);
    }

    [ClientRpc]
    public void RpcOnDeform(Vector3 impactVector, Vector3 simplifiedVector)
    {
        DeformableMesh deformableMesh = GetComponent<DeformableMesh>();
        deformableMesh.Deform(impactVector, simplifiedVector);
    }
}
