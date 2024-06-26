using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NetworkRoomManager : NetworkBehaviour
{
    /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
    /// but this code assures if the scene name changes you won't have to remember to
    /// manually update it.
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif
    [SerializeField]
    private string m_SceneName;

    public void StartNetworkRoom()
    {
        if (IsServer && !string.IsNullOrEmpty(m_SceneName) && !IsSceneLoaded("NetworkedRoomSession"))
        {
            var status = NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {m_SceneName} " +
                      $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }

    bool IsSceneLoaded(string sceneName)
    {
        // Iterate through all loaded scenes
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);

            // Check if the loaded scene has the specified name
            if (loadedScene.name == sceneName)
            {
                return true; // Scene is loaded
            }
        }

        return false; // Scene is not loaded
    }

}