using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Cutter : MonoBehaviour {

    public int hitsToCut = 4;
	public AudioSource hitSound;
    public static bool cut;
    private int hits;
    private void Start()
    {
        //hitSound = GetComponent<AudioSource>();
        hits = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if rod is attached to cutting point and if Hammer has hit it
        if (CuttingTool.rodAttached && other.tag == "Hammer") {
            if (cut)
                return;

            //play sound
            //hitSound.Play();

            if (hits++ >= hitsToCut) {
                //Execute Cut

                cut = true;
                hits = 0;

                //update mesh collider
                other.GetComponent<MeshCollider>().sharedMesh = other.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}