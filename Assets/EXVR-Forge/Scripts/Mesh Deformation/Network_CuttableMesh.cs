using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
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
            normals = md.normals,
            vertices = md.vertices,
            uv = md.uv1,
            uv2 = md.uv2,
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
            normals = mesh.normals,
            vertices = mesh.vertices,
            uv1 = mesh.uv,
            uv2 = mesh.uv2,
            name = name
        });

        return mem.GetBuffer();
    }
    
    public class MeshData
    {
        public string name;
        public int[] triangles;
        public Vector3[] vertices;
        public Vector2[] uv1;
        public Vector2[] uv2;
        public Vector3[] normals;
    }
}
