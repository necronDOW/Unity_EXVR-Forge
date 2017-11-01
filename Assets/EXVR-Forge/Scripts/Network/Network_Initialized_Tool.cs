using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class Network_Initialized_Tool : Network_Initialized
{
    public UnityEvent onPickUp;
    public UnityEvent onDetachFromHand;

    public override void InstantiatePrefab()
    {
        base.InstantiatePrefab();

        Throwable throwableScript = instantiated.GetComponent<Throwable>();
        
        throwableScript.onPickUp.AddListener(OnPickUp);
        throwableScript.onDetachFromHand.AddListener(OnDetachFromHand);
    }

    private void OnPickUp()
    {
        RpcOnPickUp();
    }

    private void OnDetachFromHand()
    {
        RpcOnDetachFromHand();
    }
    
    [ClientRpc]
    private void RpcOnPickUp()
    {
        onPickUp.Invoke();
    }

    [ClientRpc]
    private void RpcOnDetachFromHand()
    {
        onDetachFromHand.Invoke();
    }
}
