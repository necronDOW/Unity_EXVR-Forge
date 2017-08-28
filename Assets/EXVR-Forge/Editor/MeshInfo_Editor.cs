using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshInfo))]
public class MeshInfo_Editor : Editor
{
    private MeshInfo t;

    public override void OnInspectorGUI()
    {
        t = (MeshInfo)target;
    }
}
