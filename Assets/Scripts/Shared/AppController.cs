using System;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A class to bind UI events to invocations from <see cref="OptionalConnectionManager"/>, where client and host
    /// connection requests are initiated. This class also listens for status updates from Netcode for GameObjects to
    /// then display the appropriate UI elements.
    /// </summary>
    public sealed class AppController : MonoBehaviour
    {
        [SerializeField]
        NetworkManager m_NetworkManager;

        [SerializeField]
        OptionalConnectionManager m_ConnectionManager;
        

        void Awake()
        {
            m_NetworkManager = FindObjectOfType<NetworkManager>();
            DontDestroyOnLoad(this);
        }

        void Start()
        {

            
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;

        }

        void OnDestroy()
        {
            m_NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        }

        void OnClientConnected(ulong clientId)
        {

            
            // for host
            if (m_NetworkManager.IsHost)
            {
                if (clientId == m_NetworkManager.LocalClientId)
                {
                    //m_InGameUI.AddConnectionUIInstance(clientId, new int[] { }, new string[] { });
                }
                else
                {
                    // grab all loaded prefabs and represent that on the newly joined client
                    var loadedPrefabs = GetLoadedPrefabsHashesAndNames();

                }
            }
            else if (m_NetworkManager.IsClient)
            {
                // for clients that are not host
                if (clientId == m_NetworkManager.LocalClientId)
                { 
                    // grab all locally loaded prefabs and represent that on local client
                    var loadedPrefabs = GetLoadedPrefabsHashesAndNames();
                }
            }
        }

        static Tuple<int[], string[]> GetLoadedPrefabsHashesAndNames()
        {
            var loadedHashes = new int[DynamicPrefabLoadingUtilities.LoadedDynamicPrefabResourceHandles.Keys.Count];
            var loadedNames = new string[DynamicPrefabLoadingUtilities.LoadedDynamicPrefabResourceHandles.Keys.Count];
            int index = 0;
            foreach (var loadedPrefab in DynamicPrefabLoadingUtilities.LoadedDynamicPrefabResourceHandles)
            {
                loadedHashes[index] = loadedPrefab.Key.GetHashCode();
                loadedNames[index] = loadedPrefab.Value.Result.name;
                index++;
            }

            return Tuple.Create(loadedHashes, loadedNames);
        }
        
        void OnClientDisconnect(ulong clientId)
        {
        }
        
        void StartHost()
        {
            Debug.Log(nameof(StartHost));
        }
        
        void StartClient()
        {
            Debug.Log(nameof(StartClient));
        }

        public void Disconnect()
        {
            Debug.Log(nameof(Disconnect));
            m_ConnectionManager.RequestShutdown();
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}
