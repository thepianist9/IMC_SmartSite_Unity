using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class NetworkLobbyControl : NetworkBehaviour
{
    public static NetworkLobbyControl Instance;
    //private NetworkList<FixedString128Bytes> m_UsernameList;
    [SerializeField]
    public string m_InGameSceneName = "NetworkedSession";
    [SerializeField] private GameObject m_Prefab;

    [SerializeField] GameObject m_ServerTextGO;
    [SerializeField] GameObject m_ClientTextGO;


    private ServerConfig m_ServerConfig;

    // Minimum player count required to transition to next level
    [SerializeField]
    private int m_MinimumPlayerCount = 1;

    public TMP_Text LobbyText;
    private bool m_AllPlayersInLobby;


    enum UserStatus
    {
        Idle,
        Rejected,
        Ready
    }

    private Dictionary<ulong, UserStatus> SharedSpaceLobby;



    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_ServerConfig = ServerConfig.Instance;
    }
    //find networklobbycontrol game object and get the component 
    //call client ready from that script

    public override void OnNetworkSpawn()
    {
        Debug.Log("[NetworkLobbyControl]: OnNetworkSpawn");
        base.OnNetworkSpawn();
        SharedSpaceLobby = new Dictionary<ulong, UserStatus>();
        SharedSpaceLobby.Add(NetworkManager.Singleton.LocalClientId, UserStatus.Idle);


        //create a dictionary with player username, player status 
        //add self when to list with default connection status first then when client connects traverse through the connected clients from server config and
        //keep adding them with default status

        //when button is pressed send rpc to change status of the user
        //when minimum number of client have have status ready then start network shared session



        //If we are hosting, then handle the server side for detecting when clients have connected
        //and when their lobby scenes are finished loading.
        if (IsServer)
        {
            m_AllPlayersInLobby = false;

            //Server will be notified when a client connects
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene += ClientLoadedScene;
            //m_UsernameList.Add(GameClient.Instance.userName);


        }

        //Update our lobby with color status

        SetPlayerOverviewNetworkStatus();
        //SceneTransitionHandler.sceneTransitionHandler.SetSceneState(SceneTransitionHandler.SceneStates.Lobby);
    }

    private void SetPlayerOverviewNetworkStatus()
    {
        Image img;
        GameObject go;
        //server
        foreach (Transform tr in m_ServerTextGO.transform)
        {
            go = tr.gameObject;
            img = tr.GetChild(2).gameObject.GetComponent<Image>();

            switch (SharedSpaceLobby[ulong.Parse(go.name)])
            {
                case UserStatus.Idle:
                    img.color = Color.grey;
                    break;
                case UserStatus.Rejected:
                    img.color = Color.red;
                    break;
                case UserStatus.Ready:
                    img.color = Color.green;
                    break;
                default:
                    img.gameObject.SetActive(false);
                    break;
            }
        }
        //client
        foreach (Transform tr in m_ClientTextGO.transform)
        {
            go = tr.gameObject;
            img = tr.GetChild(2).gameObject.GetComponentInChildren<Image>();
            switch (SharedSpaceLobby[ulong.Parse(go.name)])
            {
                case UserStatus.Idle:
                    img.color = Color.grey;
                    break;
                case UserStatus.Rejected:
                    img.color = Color.red;
                    break;
                case UserStatus.Ready:
                    img.color = Color.green;
                    break;
                default:
                    img.gameObject.SetActive(false);
                    break;
            }
        }
    }

    /// <summary>
    ///     UpdateAndCheckPlayersInLobby
    ///     Checks to see if we have at least 2 or more people to start
    /// </summary>
    private void UpdateAndCheckPlayersInLobby()
    {
        m_AllPlayersInLobby = SharedSpaceLobby.Count >= m_MinimumPlayerCount;

        foreach (var clientLobbyStatus in SharedSpaceLobby)
        {
            SendClientReadyStatusUpdatesClientRpc(clientLobbyStatus.Key, clientLobbyStatus.Value);
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientLobbyStatus.Key))

                //If some clients are still loading into the lobby scene then this is false
                m_AllPlayersInLobby = false;
        }

        CheckForAllPlayersReady();
    }

    /// <summary>
    ///     ClientLoadedScene
    ///     Invoked when a client has loaded this scene
    /// </summary>
    /// <param name="clientId"></param>
    private void ClientLoadedScene(ulong clientId)
    {
        if (IsServer)
        {
            if (!SharedSpaceLobby.ContainsKey(clientId))
            {
                SharedSpaceLobby.Add(clientId, UserStatus.Idle);
                SetPlayerOverviewNetworkStatus();

            }
            UpdateAndCheckPlayersInLobby();
        }
    }

    /// <summary>
    ///     OnClientConnectedCallback
    ///     Since we are entering a lobby and Netcode's NetworkManager is spawning the player,
    ///     the server can be configured to only listen for connected clients at this stage.
    /// </summary>
    /// <param name="clientId">client that connected</param>
    private void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer)
        {
            if (!SharedSpaceLobby.ContainsKey(clientId))
            {
                SharedSpaceLobby.Add(clientId, UserStatus.Idle);
                SetPlayerOverviewNetworkStatus();
                UpdateAndCheckPlayersInLobby();
                
            }

        }
    }

    /// <summary>
    ///     SendClientReadyStatusUpdatesClientRpc
    ///     Sent from the server to the client when a player's status is updated.
    ///     This also populates the connected clients' (excluding host) player state in the lobby
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="isReady"></param>
    [ClientRpc]
    private void SendClientReadyStatusUpdatesClientRpc(ulong clientId, UserStatus status)
    {
        if (!IsServer)
        {
            if (!SharedSpaceLobby.ContainsKey(clientId))
                SharedSpaceLobby.Add(clientId, status);
            else
                SharedSpaceLobby[clientId] = status;

            //change status color
            SetPlayerOverviewNetworkStatus();
        }
    }

    /// <summary>
    ///     CheckForAllPlayersReady
    ///     Checks to see if all players are ready, and if so launches the game
    /// </summary>
    private void CheckForAllPlayersReady()
    {
        if (m_AllPlayersInLobby)
        {
            var allPlayersAreReady = true;
            //check for each client and update all client ready bool flag
            foreach (var clientLobbyStatus in SharedSpaceLobby)
                if (!(clientLobbyStatus.Value == UserStatus.Ready))
                    //If some clients are still loading into the lobby scene then this is false
                    allPlayersAreReady = false;

            //Only if all players are ready
            if (allPlayersAreReady)
            {
                //Remove our client connected callback
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

                //Remove our scene loaded callback
                SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene -= ClientLoadedScene;

                //Transition to the ingame scene
                GetComponent<NetworkRoomManager>().StartNetworkRoom();
            }
        }
    }

    /// <summary>
    ///     PlayerIsReady
    ///     Tied to the Ready button in the InvadersLobby scene
    /// </summary>
    public void PlayerIsReady()
    {
        SharedSpaceLobby[NetworkManager.Singleton.LocalClientId] = UserStatus.Ready;
        if (IsServer)
        {
            UpdateAndCheckPlayersInLobby();
        }
        else
        {
            OnClientIsReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        SetPlayerOverviewNetworkStatus();
    }

    /// <summary>
    ///     OnClientIsReadyServerRpc
    ///     Sent to the server when the player clicks the ready button
    /// </summary>
    /// <param name="clientid">clientId that is ready</param>
    [ServerRpc(RequireOwnership = false)]
    private void OnClientIsReadyServerRpc(ulong clientid)
    {
        if (SharedSpaceLobby.ContainsKey(clientid))
        {
            SharedSpaceLobby[clientid] = UserStatus.Ready;
            UpdateAndCheckPlayersInLobby();
            //change color status
            SetPlayerOverviewNetworkStatus();
        }
    }
}
