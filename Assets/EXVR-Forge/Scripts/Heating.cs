﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    public Color startColor;
    public Color EndColor;
    public GameObject HeatSource, WaterSource, OilSource;
    public int Heat_Detection_Accuracy;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] worldPositions;
    public float[] rodLoopTemprature;

    void Awake ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = startColor;

        HeatSource = GameObject.Find("FireLocation");
        WaterSource = GameObject.Find("WaterLocation");
        OilSource = GameObject.Find("OilLocation");

        rodLoopTemprature = new float[Heat_Detection_Accuracy];

        for (int i = 0; i < rodLoopTemprature.Length; i++)
            rodLoopTemprature[i] = 0;

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
        colors = new Color[vertices.Length];
    }

   // Initlaise colours when rod is cut

    void HeatRod()
    {
        InitRod();
        //only check every 100 points on the rod for fire collision
        int Length = (vertices.Length / Heat_Detection_Accuracy);

        for (int i = 0, j = 0; i < vertices.Length - Length && j < rodLoopTemprature.Length; i += Length, j++)
        {
            //Get world space location of this point
            vertices[i] = transform.TransformPoint(mesh.vertices[i]);
            //check fire distance
            float FireDistance = Vector3.Distance(HeatSource.transform.position, vertices[i]);
            float OilDistance = Vector3.Distance(OilSource.transform.position, vertices[i]);
            float WaterDistance = Vector3.Distance(WaterSource.transform.position, vertices[i]);

            //If in the fire
            if (FireDistance <= 0.55f)
            {
                if (rodLoopTemprature[j] < 100)
                    rodLoopTemprature[j] += Fire.temperature / 1000;
            }
            else
            {
                if (rodLoopTemprature[j] > 0)
                    rodLoopTemprature[j] -= Fire.temperature / 10000;
            }
            if (WaterDistance <= 0.2f || OilDistance <= 0.2f)
            {
                if (rodLoopTemprature[j] > 0)
                    rodLoopTemprature[j] -= Fire.temperature / 100;
            }
        }

        //loop over temprature array 
        for (int i = 0; i < rodLoopTemprature.Length ; i++)
        {
            //update colours for each temprature point
            for (int j = 0; j < Length; j++)
            {
                colors[(i * Length) + j] = Color.Lerp(startColor, EndColor, (rodLoopTemprature[i] / 100));
            }
        }
    }
}

//if (colors[i * Length].r <= 255)
//{
//    colors[i * Length + j].r = rodTemprature[i];
//}
//if (colors[i * Length].g <= 125)
//{
//    colors[i * Length + j].g = rodTemprature[i] / 10;
//}

//Color GetColorFromCurve(float temperature)
//{
//    temperature = Mathf.Clamp(temperature, 0, 100);

//    for (int i = 0; i < colorRanges.Length; i++)
//    {
//        if (temperature <= colorRanges[i].maxValue)
//        {
//            float lower = (i == 0 ? 0 : colorRanges[i - 1].maxValue);
//            float upper = colorRanges[i].maxValue;
//            float lerp = (temperature - lower) / (upper - lower);

//            return Color.Lerp((i == 0 ? Color.black : colorRanges[i - 1].color), colorRanges[i].color, lerp);
//        }
//    }

//    return Color.magenta;
//}

//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.