using UnityEngine;
using UnityEngine.Networking;

public class Network_Initialized : NetworkBehaviour
{
    public GameObject prefab;

    public override void OnStartServer()
    {
        if (isServer) {
            GameObject g = Instantiate(prefab);

            g.transform.position = transform.position;
            g.transform.rotation = transform.rotation;
            g.transform.localScale = transform.localScale;
            g.name = prefab.name;

            NetworkServer.Spawn(g);
        }
    }
}
