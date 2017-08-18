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
        if (CuttingTool.rodAttached && other.tag == "Hammer")
        {
            if (cut)
                return;

            //play sound
            //hitSound.Play();

            if (hits >= hitsToCut)
            {
                //Execute Cut
                CutRod(other.gameObject);

                cut = true;
                hits = 0;

                //update mesh collider
                other.GetComponent<MeshCollider>().sharedMesh = other.GetComponent<MeshFilter>().sharedMesh;
            }
            else
            {
                hits++;
            }          
        }
    }

    void CutRod(GameObject Rod)
    {

    }
}


  //  void Ontriggerenter (Collision Collision)
  //  {
		//if (Collision.gameObject.tag != "Rod")
		//{
  //          Debug.Log(Collision.gameObject.tag);
  //          return;
		//}

		
		//Debug.Log(Collision.gameObject.tag);
  //      GameObject victim = Collision.collider.gameObject;

  //      GameObject[] pieces = MeshCutter.MeshCut.Cut(victim, transform.position, transform.right, capMaterial);

  //      if (!pieces[1].GetComponent<Rigidbody>())
  //      {
  //          pieces[1].AddComponent<Rigidbody>();
  //          MeshCollider temp = pieces[1].AddComponent<MeshCollider>();
  //          temp.convex = true;
  //      }
          
  //     Destroy(pieces[1], 10);
  //  }

