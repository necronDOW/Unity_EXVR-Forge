using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Network_CuttableMesh : NetworkBehaviour
{
    [Command]
    public void CmdOnCut(Vector3 v1, Vector3 v2, float f)
    {
        //RpcOnCut();

        GameObject[] halves = MeshCutter.MeshCut.Cut(gameObject, v1, v2, GetComponent<CuttableMesh>().rodPrefab, f);
        
        if (halves != null) {
            for (int i = 0; i < halves.Length; i++) {
                Mesh m = halves[i].GetComponent<MeshFilter>().mesh;
                byte[] serializeMesh = SerializeMesh(m.name, m);

                NetworkServer.Spawn(halves[i]);
                halves[i].GetComponent<Network_CuttableMesh>().RpcReceiveMesh(serializeMesh);
            }
        }

        NetworkServer.Destroy(gameObject);
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
            normals = DeserializeVector3(md.normals),
            uv = DeserializeVector2(md.uv),
            uv2 = DeserializeVector2(md.uv2),
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
            normals = SerializeVector3(mesh.normals),
            uv = SerializeVector2(mesh.uv),
            uv2 = SerializeVector2(mesh.uv2),
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
        public float[,] normals;
        public float[,] uv;
        public float[,] uv2;
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
