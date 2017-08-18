using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshInfo))]
public class MeshInfo_Editor : Editor
{
    private MeshInfo t;

    public override void OnInspectorGUI()
    {
        t = (MeshInfo)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Cap Vertex Indices:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Top:  \t" + t.vxr_topCap.start + " - " + t.vxr_topCap.end, EditorStyles.miniLabel);
        EditorGUILayout.LabelField("Bottom:\t" + t.vxr_bottomCap.start + " - " + t.vxr_bottomCap.end, EditorStyles.miniLabel);
    }
}
