using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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
        SelectTransformGizmo m_selectTransformGizmo;
        [SerializeField] private GameObject SharedSpace;


        [SerializeField]
        OptionalConnectionManager m_ConnectionManager;
        ushort port = 7777;

        [SerializeField] private GameObject m_PrivateSpaceBtn;
        [SerializeField] private GameObject m_SharedSpaceBtn;


        //UI for input for network configuration
        [SerializeField] private TMP_InputField IpAddress;
        public bool privateSpace = true;

        [SerializeField] private Button HostButton;
        [SerializeField] private Button ClientButton;
        [SerializeField] private Button DisconnectButton;
        

        void Awake()
        {
            m_NetworkManager = FindObjectOfType<NetworkManager>();
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            HostButton.onClick.AddListener(StartHost);
            ClientButton.onClick.AddListener(StartClient);
            DisconnectButton.onClick.AddListener(Disconnect);
            
            m_NetworkManager.OnClientConnectedCallback += OnClientConnected;
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;

        }

        void OnDestroy()
        {
            HostButton.onClick.RemoveAllListeners();
            ClientButton.onClick.RemoveAllListeners();
            DisconnectButton.onClick.RemoveAllListeners();

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
                    if(Application.platform == RuntimePlatform.WindowsEditor)
                    {
                        Transform tr = GameObject.FindGameObjectWithTag("Private Space").transform;
                        GameObject go = Instantiate(SharedSpace, tr.position + (Vector3.right * 0.1f), tr.rotation);
                    }
;
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
            m_PrivateSpaceBtn.GetComponent<Image>().color = Color.white;
            m_SharedSpaceBtn.GetComponent<Image>().color = Color.white;
            m_selectTransformGizmo.DeactivateEdit();
        }

        public void SetUndo(bool shared)
        {
            privateSpace = shared;

            //set button color to green whichever is selected and reset other to white
            if (privateSpace)
            {
                m_PrivateSpaceBtn.GetComponent<Image>().color = Color.green;
                m_SharedSpaceBtn.GetComponent<Image>().color = Color.white;
            }
            else
            {
                m_PrivateSpaceBtn.GetComponent<Image>().color = Color.white;
                m_SharedSpaceBtn.GetComponent<Image>().color = Color.green;
            }
        }
        
        void StartHost()
        {
            //LOCAL HOST

            Debug.Log("[APP CONTROLLER]:Started Host on: " + nameof(StartHost));
            if (IpAddress.text != null)
            {
                m_ConnectionManager.StartHostIp(IpAddress.text, port);
                Debug.Log("[APP CONTROLLER]:Started Host on: " + nameof(StartHost));
            }
            else
            {
                Debug.Log("[APP CONTROLLER]: Port entered not in correct format");
            }


        }

        public void ResetTransform(int index)
        {
            if (index != null)
            { 
                if(privateSpace)
                    GameObject.FindGameObjectWithTag("Private Space").transform.GetChild(index).GetComponent<ARObject>().UndoTransform();
                else
                    GameObject.FindGameObjectWithTag("Shared Space").transform.GetChild(index).GetComponent<ARObject>().UndoTransform();
            }
        }

        void StartClient()
        {
            //START CLIENT
            ushort port = 7777;
            Debug.Log("[APP CONTROLLER]:Started Host on: " + nameof(StartClient));
            if (IpAddress.text != null)
            {
                m_ConnectionManager.StartClientIp(IpAddress.text, port);
                Debug.Log("[APP CONTROLLER]:Started Host on: " + nameof(StartClient));
            }
            else
            {
                Debug.Log("[APP CONTROLLER]: Port entered not in correct format");
            }
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
