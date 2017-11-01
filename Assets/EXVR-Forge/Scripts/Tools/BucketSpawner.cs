using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]

public class BucketSpawner : MonoBehaviour
{
    public Object prefab;
    public float spawnRate = 1;
    private Rigidbody rb;
    private GameObject Coal;

    private void Start()
    {
        rb = this.GetComponentInParent<Rigidbody>();
        Coal = new GameObject();
        Coal.name = "Coal Spawner";
    }
    void Update()
    {
        if (rb.isKinematic == true)
        {
        if ((transform.up.y * 2f) < (transform.position.y - 1f))
            SpawnPrefab();
        else ticks = 0;
        }
    }

   

    private float ticks = 0;
    private void SpawnPrefab()
    {
        ticks += Time.deltaTime;

        if (ticks >= spawnRate)
        {
            float rand = Random.Range(-0.1f, 0.1f);
            Vector3 offset = new Vector3(rand, rand, rand);

            GameObject instance = (GameObject)Instantiate(prefab, transform.position + offset, transform.rotation, Coal.transform);
            instance.GetComponentInParent<Rigidbody>().AddForce(instance.transform.up * 100f, ForceMode.Force);
            ticks = 0;
        }
    }
}
