using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]

public class Fire : MonoBehaviour
{
    public float temperature;
    public float coalIncrement = 0.025f;
    public static float maxTemprature = 100;
    public int coalLifetimeIncriment = 10000;
    public static float coalsInFire;
    private int extraTime;
    private static ParticleSystem[] flames;
    private static float[] startValues;

    private void Start()
    {
        temperature = 0f;
        coalsInFire = 0;
        GetComponent<BoxCollider>().isTrigger = true;
        extraTime = coalLifetimeIncriment;
        flames = GetComponentsInChildren<ParticleSystem>();
        startValues = new float[flames.Length];

        for (int i = 0; i < flames.Length; i++)
        {
            startValues[i] = flames[i].emission.rateOverTime.constantMax;         
        }
        SetEmissionRate(0);
    }

    private void Update()
    {
        temperature = coalsInFire;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coal")
        {
            if (coalsInFire < 100.0f)
            {
                other.GetComponent<CoalScript>().IncreaseLifetime(extraTime);
                coalsInFire += 1.0f;
                SetEmissionRate(coalsInFire);
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Coal")
        {
            other.GetComponent<CoalScript>().IncreaseLifetime(-extraTime);
            coalsInFire -= 1.0f;
            SetEmissionRate(coalsInFire);
        }
    }

    public static void SetEmissionRate(float emissionRate)
    {
        if (emissionRate > 100.0f)
            emissionRate = 100.0f;

        for (int i = 0; i < flames.Length; i++)
        {
            var rate = flames[i].emission;
            rate.rateOverTime = startValues[i] * (emissionRate / maxTemprature);
        }
    }
}
