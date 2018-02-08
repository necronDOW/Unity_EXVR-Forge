using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshInfo))]
public class CubeMeshGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GeneratorParams
    {
        [SerializeField, Range(2, 10000)] public int xSize = 2;
        [SerializeField, Range(2, 10000)] public int ySize = 2;
        [SerializeField, Range(2, 10000)] public int zSize = 2;
        [SerializeField, Range(0, 1)] public float roundness = 0.01f;
        [SerializeField] public float meshScale = 1.0f;

        public GeneratorParams() { }
        public GeneratorParams(int xSize, int ySize, int zSize, float roundness, float meshScale)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            this.zSize = zSize;
            this.roundness = roundness;
            this.meshScale = meshScale;
        }

        public GeneratorParams(GeneratorParams other)
        {
            xSize = other.xSize;
            ySize = other.ySize;
            zSize = other.zSize;
            roundness = other.roundness;
            meshScale = other.meshScale;
        }

        public GeneratorParams ApplySimplification(float simplificationFactor, bool keepRoundness = false)
        {
            GeneratorParams sGParams = new GeneratorParams();
            simplificationFactor = Mathf.Clamp01(simplificationFactor);

            sGParams.xSize = Mathf.Clamp((int)(xSize * simplificationFactor), 0, xSize);
            sGParams.ySize = Mathf.Clamp((int)(ySize * simplificationFactor), 0, ySize);
            sGParams.zSize = Mathf.Clamp((int)(zSize * simplificationFactor), 0, zSize);
            sGParams.roundness = keepRoundness ? roundness * simplificationFactor : 0.0f;
            sGParams.meshScale = meshScale * ((float)xSize / sGParams.xSize);

            if (sGParams.xSize < 2) {
                return sGParams.ApplySimplification(2.0f / sGParams.xSize, keepRoundness);
            }

            if (sGParams.ySize < 2) {
                return sGParams.ApplySimplification(2.0f / sGParams.ySize, keepRoundness);
            }

            if (sGParams.zSize < 2) {
                return sGParams.ApplySimplification(2.0f / sGParams.xSize, keepRoundness);
            }

            return sGParams;
        }
    }
    
    public static readonly GeneratorParams gParams = new GeneratorParams(5, 300, 5, 0.25f, 0.005f);

    private MeshFilter mFilter;
    private MeshCollider mCollider;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();

        if (GenerateMesh())
            Destroy(this);
    }

    private bool GenerateMesh()
    {
        Debug.Log(gParams.xSize);
        Mesh mesh = new CubeMeshFactory(gParams, name).result;
        if (mesh == null) {
            Debug.LogError("Mesh Generation failed! (name: " + name + ").");
            return false;
        }

        if (mFilter) {
            Debug.LogWarning("No mesh filter attached to CubeMeshGenerator (" + gameObject.name + ").");
            mFilter.sharedMesh = mesh;
        }

        if (mCollider) {
            Debug.LogWarning("No mesh collider attached to CubeMeshGenerator (" + gameObject.name + ").");
            mCollider.sharedMesh = mesh;
        }
        
        return true;
    }

    private void UpdateCollider()
    {
        if (mCollider) {
            mFilter.sharedMesh.RecalculateBounds();
            mCollider.sharedMesh = mFilter.sharedMesh;
        }
    }
}