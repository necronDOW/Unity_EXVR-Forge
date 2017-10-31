using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    [System.Serializable]
    public struct ColorRange
    {
        public Color color;
        public float maxValue;

        public ColorRange(Color c, float mV)
        {
            color = c;
            maxValue = mV;
        }
    }

    public ColorRange[] colorRanges;
    public Color startColor;
    public GameObject HeatSoruce;
    public int Heat_Detection_Accuracy;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] worldPositions;
    public float[] rodTemprature;

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

    void InitRod()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    Color GetColorFromCurve(float temperature)
    {
        temperature = Mathf.Clamp(temperature, 0, 100);

        for (int i = 0; i < colorRanges.Length; i++)
        {
            if (temperature <= colorRanges[i].maxValue)
            {
                float lower = (i == 0 ? 0 : colorRanges[i - 1].maxValue);
                float upper = colorRanges[i].maxValue;
                float lerp = (temperature - lower) / (upper - lower);

                return Color.Lerp((i == 0 ? Color.black : colorRanges[i - 1].color), colorRanges[i].color, lerp);
            }
        }

        return Color.magenta;
    }

    void HeatRod()
    {
        InitRod();
        //only check every 100 points on the rod for fire collision
        int Length = (vertices.Length / Heat_Detection_Accuracy);
        rodTemprature = new float[Heat_Detection_Accuracy];
        for (int i = Length; i < vertices.Length; i+= Length)
        {

            //Get world space location of this point
            vertices[i] = transform.TransformPoint(mesh.vertices[i]);
            //check fire distance
            float dist = Vector3.Distance(HeatSoruce.transform.position, vertices[i]);
            
            //If in the fire
            if (dist <= 1.2f)
            {
                rodTemprature[i] += Fire.temperature/1000;

                for (int j = 0; j < Length; j++)
                {
                    //if (colors[i - j].r <= 255)
                        //colors[i - j] = GetColorFromCurve(Fire.temperature);
                }        
            }          
            if (dist > 1.2f)
            {
             
            }
            //switch statment for multiiple colours
        }
        //only check every 100 points on the rod for fire collision
    }
}


//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.