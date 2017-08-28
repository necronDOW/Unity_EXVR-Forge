using UnityEngine;
using System.Collections;

public class WaterScript : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        //if (other.tag == "Maluable" && other.GetComponent<PerVertexScript>())
        //    other.GetComponent<PerVertexScript>().Heat(-0.05f);
    }
}
