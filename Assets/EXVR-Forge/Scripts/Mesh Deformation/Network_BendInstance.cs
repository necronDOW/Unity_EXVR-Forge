using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR.InteractionSystem;

public class Network_BendInstance : NetworkBehaviour
{
    public float minimumTimeBetweenRpc = 0.1f;
    private float timeSinceLastRpc = 0.0f;
    private BendInstance bendInstance;
    private bool isGrabbed = false;

    private void Awake()
    {
        bendInstance = GetComponent<BendInstance>();
    }

    private void Update()
    {
        if (true) {//bendInstance.isInteracting) {
            timeSinceLastRpc += Time.deltaTime;

            if (timeSinceLastRpc >= minimumTimeBetweenRpc) {
                timeSinceLastRpc = 0.0f;
                CmdOnBend(bendInstance.curvature, bendInstance.length, bendInstance.amount, bendInstance.direction);
            }
        }
    }

    [Command]
    public void CmdOnBend(float curvature, float length, float amount, bool direction)
    {
        RpcOnBend(curvature, length, amount, direction);
    }

    [ClientRpc]
    public void RpcOnBend(float curvature, float length, float amount, bool direction)
    {
        bendInstance.curvature = curvature;
        bendInstance.length = length;
        bendInstance.amount = amount;
        bendInstance.direction = direction;
    }
}
