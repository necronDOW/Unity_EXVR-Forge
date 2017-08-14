using UnityEngine;
using System.Collections;

public class CoalScript : MonoBehaviour
{
    public int startLifetime = 1000;
    private int lifetime;
    private bool inFire;

    void Start()
    {
        lifetime = startLifetime;
    }

    void Update()
    {
       ReduceLifetime(1);

       if (lifetime <= 0)
       {
            if (inFire && Fire.coalsInFire > 0)
            {
                Fire.coalsInFire -= 1;
                Fire.SetEmissionRate(Fire.coalsInFire);
            }
            Destroy(gameObject, 0.05f);
       }      
    }

    public void IncreaseLifetime(int amount)
    {
        lifetime += amount + Random.Range(-200, 200);
        inFire = true;
    }

    public void ReduceLifetime(int amount)
    {
        lifetime -= amount;
    }
}