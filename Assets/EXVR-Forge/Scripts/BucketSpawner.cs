using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]

public class BucketSpawner : MonoBehaviour
{
    public Object prefab;
    public float spawnRate = 1;

    void Update()
    {
        if ((transform.up.y * 2f) < (transform.position.y - 1f))
            SpawnPrefab();
        else ticks = 0;
    }

    private float ticks = 0;
    private void SpawnPrefab()
    {
        ticks += Time.deltaTime;

        if (ticks >= spawnRate)
        {
            float rand = Random.Range(-0.1f, 0.1f);
            Vector3 offset = new Vector3(rand, rand, rand);

            GameObject instance = (GameObject)Instantiate(prefab, transform.position + offset, transform.rotation);
            instance.GetComponent<Rigidbody>().AddForce(instance.transform.up * 100f, ForceMode.Force);
            ticks = 0;
        }
    }
}
