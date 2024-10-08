using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerNetworkSpawner : NetworkBehaviour
{
    private GameObject _player;


    public void Awake()
    {
        if (IsLocalPlayer)
        {
            SetSpawnedNetworkObject();
        }
        else
        {
            return;
        }
    }

    void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        SetSpawnedNetworkObject();
    }

    void SetSpawnedNetworkObject()
    {
        Debug.Log($"Searching for: Player_{NetworkManager.LocalClientId}" );
        _player = GameObject.Find("Player_" + NetworkManager.LocalClientId.ToString());

        if(_player is not null) _player.GetComponent<ClientPlayerMove>().SetCamera();
    }

    private void Update()
    {
        if(_player is null)
        {
            SetSpawnedNetworkObject();
        }
    }
    private void OnApplicationQuit()
    {
        NetworkManager.Singleton.Shutdown();
    }
}

