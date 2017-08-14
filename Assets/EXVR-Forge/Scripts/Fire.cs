using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]

public class Fire : MonoBehaviour
{
    public float temperature;
    public float coalIncrement = 0.025f;
    public float maxTemprature = 100;
    public int coalLifetimeIncriment = 10000;
    public static int coalsInFire;
    private int extraTime;
    private static ParticleSystem[] flames;

    private void Start()
    {
        temperature = 0f;
        coalsInFire = 0;
        GetComponent<BoxCollider>().isTrigger = true;
        extraTime = coalLifetimeIncriment;
        flames = GetComponentsInChildren<ParticleSystem>();
        //get all the original values
        //create scalar value to be applied
        SetEmissionRate(0);
    }

    private void Update()
    {
        //temperature = coalsInFire;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coal")
        {
            if (coalsInFire < 100)
            {
                other.GetComponent<CoalScript>().IncreaseLifetime(extraTime);
                coalsInFire += 1;
                SetEmissionRate(coalsInFire);
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Coal")
        {
            other.GetComponent<CoalScript>().IncreaseLifetime(-extraTime);
            coalsInFire -= 1;
            SetEmissionRate(coalsInFire);
        }
    }

    public static void SetEmissionRate(int emissionRate)
    {
        if (emissionRate > 100)
            emissionRate = 100;

        for (int i = 0; i < flames.Length; i++)
        {
            var rate = flames[i].emission;
            rate.rateOverTime = emissionRate;
        }
    }
}
