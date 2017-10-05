using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    public Color startColor;
    public GameObject HeatSoruce;
    public int Heat_Detection_Accuracy;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] worldPositions;

    void Awake ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = startColor;

        mesh.colors = colors;
    }


    void Update()
    {
        //Initialize rod
        if (vertices.Length == 0)
            Awake();

        HeatRod();

        //update colours
        mesh.colors = colors;
    }

    void HeatRod()
    {
        //only check every 100 points on the rod for fire collision
        int Length = (vertices.Length / Heat_Detection_Accuracy);
        for (int i = Length; i < vertices.Length; i+= Length)
        {
            //Get world space location of this point
            vertices[i] = transform.TransformPoint(mesh.vertices[i]);
            //check fire distance
            float dist = Vector3.Distance(HeatSoruce.transform.position, vertices[i]);
          
            if (dist <= 1.2f)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (colors[i - j].r < 255)
                }        
            }          
            if (dist > 1.2f)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (colors[i - j].r > 0)
                }
            }
            //switch statment for multiiple colours
        }
        //only check every 100 points on the rod for fire collision
    }
}
//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.