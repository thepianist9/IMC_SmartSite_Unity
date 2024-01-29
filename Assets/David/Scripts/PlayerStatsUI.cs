using StarterAssets;
using System;

using TMPro;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class PlayerStatsUI : NetworkBehaviour
{

    public static PlayerStatsUI Singleton { get; private set; }
    private LobbyControl lobbyControl;
    //Overview Panel Prefabs
    [SerializeField] GameObject m_TMPPrefab;
    [SerializeField] GameObject m_ServerTextGO;
    [SerializeField] GameObject m_ClientTextGO;
    [SerializeField] GameObject ChatGO;
    [SerializeField] Toggle _toggle;
    [SerializeField] TMP_Text _PlayerPanel;

    [SerializeField] GameObject m_BtnGroup;
    [SerializeField] ServerConfig m_ServerConfig;
    private ThirdPersonController tpc;

    // Start is called before the first frame update

    void OnEnable()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(PlayerStatsUI)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;

        lobbyControl = LobbyControl.Instance;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }



    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        if (scene.name == "OfflineSession")
        {
            Client LocalClient = ServerCheck.Instance.LocalClient;
            DisplayOfflinePanel();
        }
        else if(scene.name == "NetworkedSession")
        {
            
            _toggle.onValueChanged.AddListener(ToggleNetworkChat);
            
        }
    }


    private void ToggleNetworkChat(bool isOn)
    {
        tpc = GameObject.Find("Player_" + NetworkManager.Singleton.LocalClientId).GetComponent<ThirdPersonController>();
        ChatGO.SetActive(isOn);
        tpc.enabled = !isOn;
    }


    private void DisplayOfflinePanel( )
    {
        _PlayerPanel.text = $"PC Name: {ServerCheck.Instance.m_PcName}";
    }


    public void UpdateOnlinePanel()
    {

        //Spawn TMP prefab in Server/Client slots
        //udpdate client prefab inforamtion

        if (m_TMPPrefab is not null && m_ServerTextGO is not null && m_ClientTextGO is not null)
        {
            //Recycle overview Panel
            RecycleOverviewPanel();
            //TODO: FIND server config and get info from other clients

            foreach (var clientID in m_ServerConfig.connectedClients)
            {
                GameClient gc = GameObject.Find("Player_"+clientID).GetComponent<GameClient>();
                if(gc is not null)
                {
                    
                    Debug.Log(gc.clientId.Value);
                    if (m_ServerConfig.clientID.Value == gc.clientId.Value)
                    {
                        //Add to server slot in panel
                        GameObject go = GameObject.Instantiate(m_TMPPrefab, m_ServerTextGO.transform);
                        go.name = gc.clientId.Value.ToString();
                        go.transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.gray;

                        go.GetComponentInChildren<TMP_Text>().text = $"UserName: {gc.userName.Value}  Net. ID: {gc.clientId.Value}";
                        if (gc.clientId.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            go.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                            
                        }
                    }
                    //Add to client slots in panel
                    else
                    {
                        GameObject go = GameObject.Instantiate(m_TMPPrefab, m_ClientTextGO.transform);
                        go.name = gc.clientId.Value.ToString();
                        go.transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.gray;

                        go.GetComponentInChildren<TMP_Text>().text = $"UserName: {gc.userName.Value}  Net. ID: {gc.clientId.Value}";
                        if (gc.clientId.Value == NetworkManager.Singleton.LocalClientId)
                        {
                            go.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }

            }
        }
    }

    private void RecycleOverviewPanel()
    {
        foreach(Transform tr in m_ServerTextGO.transform)
        {
            Destroy(tr.gameObject);
        }foreach(Transform tr in m_ClientTextGO.transform)
        {
            Destroy(tr.gameObject);
        }
    }
    
  


    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Scene Unloaded: " + scene.name);
        if (scene.name == "NetworkedSession")
        {
            /*NetworkManager.Singleton.OnClientConnectedCallback -= UpdateOnlinePanel;
            NetworkManager.Singleton.OnClientDisconnectCallback -= UpdateOnlinePanel;*/
        }
    }

    public void Exit()
    {
        // Clean up your NetworkBehaviour
        SceneTransitionHandler.sceneTransitionHandler.ExitAndLoadStartMenu();
        NetworkSystemControl.Singleton.IsOnline = false;

        if(NetworkSystemControl.Singleton.IsOnline)
        {
            base.OnDestroy();
        }
        // Always invoked the base 
        
    }

    public void ToggleBtn(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }


    public void SetOnlineMode()
    {
        NetworkSystemControl.Singleton.IsOnline = true;
        SceneTransitionHandler.sceneTransitionHandler.ExitAndLoadStartMenu();
    }

}
