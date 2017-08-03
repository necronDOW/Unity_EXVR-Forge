using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PortCullis : MonoBehaviour
{
    public string targetSceneName;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            NetworkManager.singleton.ServerChangeScene(targetSceneName);
    }
}
