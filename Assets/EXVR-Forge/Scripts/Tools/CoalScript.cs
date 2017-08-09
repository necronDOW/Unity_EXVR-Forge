using UnityEngine;
using System.Collections;

public class CoalScript : MonoBehaviour
{
    public int startLifetime = 1;

    private int lifetime;

    void Start()
    {
        lifetime = startLifetime;
    }

    void Update()
    {
        ReduceLifetime(100);

        if (lifetime <= 0)
            Destroy(gameObject, 0.05f);
    }

    public void IncreaseLifetime(int amount)
    {
        lifetime += amount;
        if (lifetime > startLifetime)
            lifetime = startLifetime;
    }

    public void ReduceLifetime(int amount)
    {
        lifetime -= amount;
    }
}