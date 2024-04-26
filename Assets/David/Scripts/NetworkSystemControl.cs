using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP; 
using UnityEngine;

public class NetworkSystemControl : MonoBehaviour

{
    [SerializeField]
    private string m_LobbySceneName = "SmartSiteLobby";
    private string m_OfflineSceneName = "OfflineSession";
    [SerializeField] private string m_NetworkedSession = "NetworkedSession";



    public bool IsOnline = false;
    public string m_ClientType;
    public string m_userName;
    public string m_pcName;
    public string m_clientIP;
    public ushort m_port = 7777;

    [SerializeField] private int m_MaxPlayers = 10;
    public static NetworkSystemControl Singleton; 

    private void Awake()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(NetworkSystemControl)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
        DontDestroyOnLoad(gameObject);
        
    }
    private void Start()
    {
        
    }

    

   

    public void StartGame()
    {
        if(IsOnline) 
        {
            UIManager.Instance.SetIPAddr();
            StartSessionOnline();
        }
        else
        {
            StartSessionOffline();
        }
        
    }
    public void StartSessionOffline()
    {
        if (SceneTransitionHandler.sceneTransitionHandler != null)
        {
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_OfflineSceneName);
        }
    }

    public void StartSessionOnline()
    {
        var utpTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (utpTransport)
        {
            if (ServerCheck.Instance.IsServer)
            {
                //set network manager server IP
                utpTransport.SetConnectionData(Sanitize(ServerCheck.Instance.server.ipaddress.ToString()), m_port, "0.0.0.0");
                Debug.Log($"Server started with IP Address: {utpTransport.ConnectionData.Address}, {utpTransport.ConnectionData.ToString()}, listening on: 0.0.0.0");

                if (NetworkManager.Singleton.StartHost())
                {
                    SceneTransitionHandler.sceneTransitionHandler.RegisterCallbacks();
                    //Remove local tpp
                    GameObject.Find("PlayerArmature_Offline").SetActive(false);
                    SceneTransitionHandler.sceneTransitionHandler.AddScene(m_NetworkedSession);
                    Debug.Log($"Host started at: {m_clientIP}");
                }
                else
                {
                    Debug.LogError("Failed to start host.");
                }
            }

            else
            {

                //Get IP from UI Manager and assign to UTP Transport
                utpTransport.SetConnectionData(Sanitize(ServerCheck.Instance.server.ipaddress.ToString()), m_port);

                Debug.Log($"[NetworkSystemControl]:: Connecting to {ServerCheck.Instance.server.ipaddress} with port: {m_port}");
                //start client
                if (!NetworkManager.Singleton.StartClient())
                {
                    Debug.LogError("Failed to start client.");
                }
                else
                {
                    GameObject.Find("PlayerArmature_Offline").SetActive(false);
                    Debug.Log($"Client started at: {m_clientIP}");
                }
            }
        }
        else Debug.Log("No Network Transport config found");
    }

  

    static string Sanitize(string dirtyString)
    {
        // sanitize the input for the ip address
        return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
    } 

}
