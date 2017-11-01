using UnityEngine;
using UnityEngine.Networking;

public class Network_Initialized : NetworkBehaviour
{
    public GameObject prefab;
    protected GameObject instantiated;

    public override void OnStartServer()
    {
        if (isServer) {
            InstantiatePrefab();
            NetworkServer.Spawn(instantiated);
        }
    }

    public virtual void InstantiatePrefab()
    {
        instantiated = Instantiate(prefab);

        instantiated.transform.position = transform.position;
        instantiated.transform.rotation = transform.rotation;
        instantiated.name = prefab.name;
    }
}
