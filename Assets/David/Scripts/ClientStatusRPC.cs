using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientStatusRPC : MonoBehaviour
{
    [SerializeField] TMP_Text clientId;
    [SerializeField] Image selfIcon;
    public void init(string userId, bool self)
    {
        clientId.text = userId;
        selfIcon.enabled = self;
    }
    // Start is called before the first frame update
/*    public void ReadyPlayer()
    {
        Debug.Log("Ready Player called");
        
        if(gameObject.name == clientId && SceneManager.GetActiveScene().name == "NetworkedSession")
        {
            Debug.Log("ClientReady!!!!!");
            NetworkLobbyControl.Instance.PlayerIsReady();
        }
    }*/

   

}
