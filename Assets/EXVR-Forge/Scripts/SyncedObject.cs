using UnityEngine;
using UnityEngine.Networking;

public class SyncedObject : NetworkBehaviour
{
    [SerializeField, Range(1, 29)] private int sendRate = 9;
    [SerializeField] private bool syncScale = false;

    private float timer = 0.0f;
    private float timeBetweenSends = 0.0f;
    private Vector3 newPosition, newScale;
    private Quaternion newRotation;

    void Start()
    {
        timeBetweenSends = 60.0f / (float)sendRate;
	}
    
	void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenSends)
        {
            if (SendTransformData())
                timer = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (NetworkServer.active)
        {
            transform.position = newPosition;
            transform.rotation = newRotation;
            transform.localScale = newScale;
        }
    }

    private bool SendTransformData()
    {
        CmdTranslate(transform.position, transform.rotation);

        if (syncScale)
            CmdScale(transform.localScale);

        return true;
    }

    [Command]
    private void CmdTranslate(Vector3 pos, Quaternion rot)
    {
        newPosition = pos;
        newRotation = rot;
    }

    [Command]
    private void CmdScale(Vector3 scale)
    {
        newScale = scale;
    }
}
