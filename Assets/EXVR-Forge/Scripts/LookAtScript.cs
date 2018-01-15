using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{

    public static Vector3 nullTarget = new Vector3(int.MinValue, int.MinValue, int.MinValue);
    public Transform targetTransform;
    public Vector3 target = nullTarget;

    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (targetTransform)
            target = targetTransform.position;

        if (target != nullTarget) {
            Vector3 targetPoint = new Vector3(transform.position.x, target.y, target.z) - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(-targetPoint, Vector3.right);

            transform.rotation = originalRotation;
            transform.rotation *= Quaternion.Euler(0, 0, 90);// * Quaternion.Euler(90, 0, 0);
            transform.rotation *= targetRotation;
        }
    }
}
