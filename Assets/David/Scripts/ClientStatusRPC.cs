using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientStatusRPC : MonoBehaviour
{
    string clientId;
    private void Start()
    {
        clientId = NetworkManager.Singleton.LocalClientId.ToString();
        if (gameObject.name != clientId)
        {
            GetComponentInChildren<Button>().enabled = false;
        }
    }
    // Start is called before the first frame update
    public void ReadyPlayer()
    {
        Debug.Log("Ready Player called");
        
        if(gameObject.name == clientId && SceneManager.GetActiveScene().name == "NetworkedSession")
        {
            Debug.Log("ClientReady!!!!!");
            NetworkLobbyControl.Instance.PlayerIsReady();
        }
    }

   

}
