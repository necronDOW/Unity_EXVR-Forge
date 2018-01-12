using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    public enum ForwardAxis
    {
        X,
        Y,
        Z
    }

    public static Vector3 nullTarget = new Vector3(int.MinValue, int.MinValue, int.MinValue);
    public Vector3 target = nullTarget;
    public bool xAxis = true;
    public bool yAxis = true;
    public bool zAxis = true;
    public ForwardAxis forwardAxis = ForwardAxis.Z;

    private void Update()
    {
        if (target != nullTarget) {
            Vector3 relativePosition = target - transform.position;
            relativePosition = new Vector3(
                xAxis ? 0 : relativePosition.x,
                yAxis ? 0 : relativePosition.y,
                zAxis ? 0 : relativePosition.z);

            Quaternion allAxesLookAt = Quaternion.LookRotation(relativePosition, transform.up);
            TranslateRelativeToForwardAxis(ref allAxesLookAt);
            transform.eulerAngles = allAxesLookAt.eulerAngles;
        }
    }

    private void TranslateRelativeToForwardAxis(ref Quaternion original)
    {
        switch (forwardAxis) {
            case ForwardAxis.X:
                original *= Quaternion.Euler(0.0f, 90.0f, 0.0f);
                break;
            case ForwardAxis.Y:
                original *= Quaternion.Euler(90.0f, 0.0f, 0.0f);
                break;
            default: case ForwardAxis.Z:
                break;
        }
    }
}
