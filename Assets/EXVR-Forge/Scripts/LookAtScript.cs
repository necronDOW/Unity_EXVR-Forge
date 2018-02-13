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
            Vector3 targetPos = target;
            targetPos.x = transform.position.x;
            transform.LookAt(targetPos);
        }
    }
}
