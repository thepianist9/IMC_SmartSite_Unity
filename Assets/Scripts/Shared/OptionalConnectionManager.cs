using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A state machine to track a client's connection state.
    /// Note: this class is not absolutely necessary for dynamic prefab loading. It is simply added to for simpler
    /// navigation of code through the different states of connection, particularly client reconnection and late join.
    /// support. For more details, see the <see cref="ConnectionApprovalRequiredForLateJoining"/> use-case.
    /// </summary>
    public class OptionalConnectionManager : MonoBehaviour
    {
        [SerializeField]
        internal NetworkManager m_NetworkManager;        
        [SerializeField]
        private LobbyChatBehavior m_NetworkChat;
        [SerializeField]
        private GameObject SeverPanel; 
        [SerializeField]
        private GameObject ClientPanel;
        [SerializeField]
        private GameObject ClientInfoPrefab;


        ConnectionState m_CurrentState;
        
        internal OfflineState m_Offline;
        
        internal ClientConnectingState m_ClientConnecting;
        
        internal ClientConnectedState m_ClientConnected;
        
        internal ClientPreloadingState m_ClientPreloading;
        
        internal StartingHostState m_StartingHost;
        
        internal HostingState m_Hosting;
        
        internal string m_ConnectAddress;
        
        internal ushort m_Port;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            m_Offline = new OfflineState(this);
            m_ClientConnecting = new ClientConnectingState(this);
            m_ClientConnected = new ClientConnectedState(this);
            m_ClientPreloading = new ClientPreloadingState(this);
            m_StartingHost = new StartingHostState(this);
            m_Hosting = new HostingState(this);
            
            m_CurrentState = m_Offline;

            m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            m_NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            m_NetworkManager.OnServerStarted += OnServerStarted;
            m_NetworkManager.OnTransportFailure += OnTransportFailure;
        }
        
        void OnDestroy()
        {
            m_NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
            m_NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            m_NetworkManager.OnServerStarted -= OnServerStarted;
            m_NetworkManager.OnTransportFailure -= OnTransportFailure;
        }
        
        void OnClientConnectedCallback(ulong clientId)
        {
            m_CurrentState.OnClientConnected(clientId);
            RefreshClientOverviewPanel();
            if (m_NetworkManager.LocalClientId == clientId)
                m_NetworkChat.ToggleChat(true);
        }

        void RefreshClientOverviewPanel()
        {
            List<ulong> connectedClients = new List<ulong>(m_NetworkManager.ConnectedClientsIds);

            //destroy all children of server and client panel
            foreach (Transform child in SeverPanel.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in ClientPanel.transform)
            {
                Destroy(child.gameObject);
            }

            connectedClients.ForEach(clientId =>
            {
                bool self = m_NetworkManager.LocalClientId == clientId;
                if (clientId == 0)
                {
                    GameObject client = Instantiate(ClientInfoPrefab, SeverPanel.transform);
                    client.GetComponent<ClientStatusRPC>().init("Client_" + clientId, self);
                }
                else
                {
                    GameObject client = Instantiate(ClientInfoPrefab, ClientPanel.transform);
                    client.GetComponent<ClientStatusRPC>().init("Client_" + clientId, self);
                }
            });

        }
        
        void OnClientDisconnectCallback(ulong clientId)
        {
            m_CurrentState.OnClientDisconnect(clientId);
            RefreshClientOverviewPanel();
            if (m_NetworkManager.LocalClientId == clientId)
                m_NetworkChat.ToggleChat(false);
        }
        
        void OnServerStarted()
        {
            m_CurrentState.OnServerStarted();
        }
        
        void OnTransportFailure()
        {
            m_CurrentState.OnTransportFailure();
        }

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {m_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

            if (m_CurrentState != null)
            {
                m_CurrentState.Exit();
            }
            m_CurrentState = nextState;
            m_CurrentState.Enter();
        }

        public void StartClientIp(string ipaddress, ushort port)
        {
            m_ConnectAddress = ipaddress;
            m_Port = port;
            m_CurrentState.StartClientIP(ipaddress, port);
        }

        public void StartHostIp(string ipaddress, ushort port)
        {
            m_ConnectAddress = ipaddress;
            m_Port = port;
            m_CurrentState.StartHostIP(ipaddress, port);
        }

        public void RequestShutdown()
        {
            m_CurrentState.OnUserRequestedShutdown();
        }

    }
}