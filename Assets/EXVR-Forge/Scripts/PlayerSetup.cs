using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable;

    private Camera sceneCamera;

    private void Start()
    {
        sceneCamera = Camera.main;

        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
                componentsToDisable[i].enabled = false;
        }
        else
        {
            if (sceneCamera)
                sceneCamera.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (sceneCamera)
            sceneCamera.gameObject.SetActive(true);
    }
}
