using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network_Initialized : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    public SpawnableObject[] initialSpawn;

    private void Awake()
    {
        if (Network.isServer) {
            foreach (SpawnableObject o in initialSpawn) {
                GameObject g = Instantiate(o.prefab);

                g.transform.position = o.position;
                g.transform.eulerAngles = o.rotation;
                g.transform.localScale = o.scale;

                NetworkServer.Spawn(g);
            }
        }
    }
}
