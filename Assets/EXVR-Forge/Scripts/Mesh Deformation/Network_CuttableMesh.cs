using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnCut(Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        Mesh[] mHalves = MeshCutter.MeshCut.Cut(gameObject, anchorPoint, normalDirection, distanceLimit);

        GameObject[] halves = new GameObject[2];
        for (int i = 0; i < halves.Length; i++) {
            halves[i] = InstantiateCutInstance(GetComponent<CuttableMesh>().rodPrefab, transform);
            halves[i].GetComponent<MeshFilter>().mesh = mHalves[i];
            halves[i].GetComponent<DeformableBase>().SplitCollider(anchorPoint, i, mHalves[i].vertices);
            halves[i].GetComponent<MeshStateHandler>().ChangeState(false);

            NetworkServer.Spawn(halves[i]);
        }

        NetworkInstanceId[] nids = halves.Select(h => h.GetComponent<NetworkIdentity>().netId).ToArray();
        RpcOnCut(transform.position, transform.rotation, nids, anchorPoint, normalDirection, distanceLimit);

        NetworkServer.Destroy(gameObject);
    }
    
    [ClientRpc]
    private void RpcOnCut(Vector3 syncPosition, Quaternion syncRotation, NetworkInstanceId[] halfIds, Vector3 anchorPoint, Vector3 normalDirection, float distanceLimit)
    {
        GameObject[] halfObjects = new GameObject[halfIds.Length];
        for (int i = 0; i < halfObjects.Length; i++)
            halfObjects[i] = ClientScene.FindLocalObject(halfIds[i]);

        gameObject.transform.position = syncPosition;
        gameObject.transform.rotation = syncRotation;

        Mesh[] halves = MeshCutter.MeshCut.Cut(gameObject, anchorPoint, normalDirection, distanceLimit);

        for (int i = 0; i < halfObjects.Length; i++) {
            halfObjects[i].GetComponent<MeshFilter>().mesh = halves[i];
            halfObjects[i].GetComponent<MeshCollider>().sharedMesh = halves[i];
            halfObjects[i].GetComponent<MeshStateHandler>().ChangeState(false);
        }
    }

    private GameObject InstantiateCutInstance(GameObject original, Transform transform, Mesh mesh = null)
    {
        GameObject obj = GameObject.Instantiate(original, transform.position, transform.rotation);
        obj.transform.localScale = transform.localScale;

        return obj;
    }

    [ClientRpc]
    public void RpcReceiveMesh(byte[] data)
    {
        BinaryFormatter fmt = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();
        MeshData md = fmt.Deserialize(mem) as MeshData;

        Mesh mesh = new Mesh {
            triangles = md.triangles,
            vertices = DeserializeVector3(md.vertices),
            //normals = DeserializeVector3(md.normals),
            uv = DeserializeVector2(md.uv),
            //uv2 = DeserializeVector2(md.uv2),
            name = md.name
        };

        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private byte[] SerializeMesh(string name, Mesh mesh)
    {
        MemoryStream mem = new MemoryStream();
        BinaryFormatter fmt = new BinaryFormatter();

        fmt.Serialize(mem, new MeshData {
            triangles = mesh.triangles,
            vertices = SerializeVector3(mesh.vertices),
            //normals = SerializeVector3(mesh.normals),
            uv = SerializeVector2(mesh.uv),
            //uv2 = SerializeVector2(mesh.uv2),
            name = name
        });

        return mem.GetBuffer();
    }
    
    [System.Serializable]
    public class MeshData
    {
        public string name;
        public int[] triangles;
        public float[,] vertices;
        //public float[,] normals;
        public float[,] uv;
        //public float[,] uv2;
    }

    private float[,] SerializeVector3(Vector3[] v)
    {
        float[,] s = new float[v.Length,3];

        for (int i = 0; i < v.Length; i++) {
            s[i,0] = v[i].x;
            s[i, 1] = v[i].y;
            s[i, 2] = v[i].z;
        }

        return s;
    }

    private Vector3[] DeserializeVector3(float[,] s)
    {
        Vector3[] v = new Vector3[s.GetLength(0)];

        for (int i = 0; i < v.Length; i++) {
            v[i].x = s[i, 0];
            v[i].y = s[i, 1];
            v[i].z = s[i, 2];
        }

        return v;
    }

    private float[,] SerializeVector2(Vector2[] v)
    {
        float[,] s = new float[v.Length, 2];

        for (int i = 0; i < v.Length; i++) {
            s[i, 0] = v[i].x;
            s[i, 1] = v[i].y;
        }

        return s;
    }

    private Vector2[] DeserializeVector2(float[,] s)
    {
        Vector2[] v = new Vector2[s.GetLength(0)];

        for (int i = 0; i < v.Length; i++) {
            v[i].x = s[i, 0];
            v[i].y = s[i, 1];
        }

        return v;
    }
}
