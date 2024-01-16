
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ServerConfig:NetworkBehaviour
{

    public static ServerConfig Instance;

    [SerializeField] public NetworkVariable<ulong> clientID;
    [SerializeField] public NetworkVariable<FixedString32Bytes> pcName;
    [SerializeField] public NetworkVariable<FixedString32Bytes> type;
    [SerializeField] public NetworkVariable<FixedString32Bytes> ipAddress; 
    [SerializeField] public NetworkList<ulong> connectedClients = new NetworkList<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public List<ulong> clientList = new List<ulong>();

    private void Awake()
    {
        Instance = this;
        //Initialize only on awake and not on declaration
        connectedClients = new NetworkList<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
       

    }

   

    private void Start()
    {
        //Setup server independant of network connection (not yet connected at this point)
        if(IsServer)
        {
            ServerCheck sc = ServerCheck.Instance;
            SetServer(sc.m_PcName, sc.m_ClientIP, sc.m_ServerType);
        }
    }

    void Update()
    {
        //This is just an example that shows how to add an element to the list after its initialization:
        if (!IsServer) { return; } //remember: only the server can edit the list
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Debug.Log("adding new client to list");
            connectedClients.Add(1);
        }
    }



    public override void OnNetworkSpawn()
    {
        
        //adding callbacks to manage connected client list
        if (IsServer)
        {
            clientID.Value = NetworkManager.Singleton.LocalClientId;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            connectedClients.OnListChanged += OnClientListChanged;

            //Add self
            connectedClients.Add(NetworkManager.Singleton.LocalClientId);
            //connectedClients.Add(); //if you want to initialize the list with some default values, this is a good time to do so.
        }
        //Add listener delegates to update clientlist on client side
        else
        {

            connectedClients.OnListChanged += OnClientListChanged;
            
        }

        //Display OverviewPanel
        //clientList.Add(NetworkManager.Singleton.LocalClientId);
        PlayerStatsUI.Singleton.UpdateOnlinePanel();

    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log("[ServerConfig: OnclientConnectedCallback]: client connected: " + clientId);

        connectedClients.Add(clientId);
        if (!clientList.Contains(clientId))
        {
            clientList.Add(clientId);
        }

    }
    private void OnClientDisconnectCallback(ulong obj)
    {
        connectedClients.Remove(obj);
        clientList.Remove(obj);
    }

    private void OnClientListChanged(NetworkListEvent<ulong> changeEvent)
    {
        //Display updated client list
        PlayerStatsUI.Singleton.UpdateOnlinePanel();
        
    }



        internal void SetServer(string pcName, string ipAddress, string type)
    {
        this.pcName.Value = pcName;
        this.type.Value = type;
        this.ipAddress.Value = ipAddress;       
    }

  
}