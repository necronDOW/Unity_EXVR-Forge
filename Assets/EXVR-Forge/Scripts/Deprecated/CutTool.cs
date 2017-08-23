using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CutTool : MonoBehaviour
{
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
         if (other.tag == "Hammer") //CuttingTool.rodAttached &&
         {
             if (cut)
                 return;
 
             //play sound
             //hitSound.Play();
 
             if (hits++ >= hitsToCut)
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