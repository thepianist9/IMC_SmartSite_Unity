using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    [SerializeField] private GameObject _ServerPrefab;

    [SerializeField] private GameObject m_ServerButtonGroup;
    [SerializeField] private GameObject m_ServerButtonPrefab;

    [SerializeField] private TMP_InputField m_NetworkType;
    [SerializeField] private TMP_InputField m_PcName;
    [SerializeField] private GameObject m_ConfigPanel;
    [SerializeField] private GameObject m_UsernameGameObject; //
    [SerializeField] private TMP_InputField m_UsernameIPField; //
    [SerializeField] private TMP_InputField m_IPv4AddIPField; //
    [SerializeField] private TMP_Text m_SubtitleText;
    [SerializeField] private TMP_Text m_TitleText;

    [SerializeField] private GameObject m_ServerNetworkType;
    [SerializeField] private GameObject m_IP;


    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(UIManager)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Instance = this;
    }
    private void Start()
    {

        m_PcName.text = ServerCheck.Instance.m_PcName;
        m_IPv4AddIPField.text = ServerCheck.Instance.m_ClientIP;

        //Switch UI based on whether client is offline or online
        SwitchUI();
    }

    public void DisplayClientMenu(List<Server> serverList)
    {

        //change config title to client 
        m_SubtitleText.text = "GameClient Config";
        NetworkSystemControl.Singleton.m_ClientType = "GameClient";
        //display button for each server in server list 
        foreach (Server server in serverList)
        {
            GameObject _ServerButton = Instantiate(_ServerPrefab, m_ServerButtonGroup.transform);

            Image statusImg = _ServerButton.GetComponent<Image>();
            _ServerButton.GetComponent<Button>().onClick.AddListener(() => { ServerClicked(statusImg, server); });
            _ServerButton.GetComponent<Server>().SetServer(server.pcName, server.ipAddress, server.Type);
            _ServerButton.GetComponentInChildren<TMP_Text>().text = server.pcName;
        }

    }

    public void DisplayServerMenu(Server server)
    {
        GameObject _ServerButton = Instantiate(_ServerPrefab, m_ServerButtonGroup.transform);
        Image statusImg = _ServerButton.GetComponent<Image>();
        _ServerButton.GetComponent<Button>().onClick.AddListener(() => { ServerClicked(statusImg, server); });
        _ServerButton.GetComponent<Server>().SetServer(server.pcName, server.ipAddress, server.Type);
        _ServerButton.GetComponentInChildren<TMP_Text>().text = server.pcName;
        //Spawn Server Button on top of menu 
        //Display server info in menu
    }

    public void SwitchUI()
    {
        if (NetworkSystemControl.Singleton.IsOnline)
        {
            m_TitleText.text = "Online Mode";

            //Set client ui menu
            if (ServerCheck.Instance.IsServer)
            {
                m_SubtitleText.text = "Server";
            }
            else
            {
                m_SubtitleText.text = "GameClient";
            }
        }
        else
        {
            m_TitleText.text = "Offline Mode";

            m_SubtitleText.gameObject.SetActive(NetworkSystemControl.Singleton.IsOnline);
            m_UsernameGameObject.SetActive(NetworkSystemControl.Singleton.IsOnline);
            m_ServerButtonGroup.gameObject.SetActive(NetworkSystemControl.Singleton.IsOnline);
        }

        m_ServerNetworkType.SetActive(NetworkSystemControl.Singleton.IsOnline);
        m_IP.SetActive(NetworkSystemControl.Singleton.IsOnline);
    } 




    private void ServerClicked(Image statusImg, Server server)
    {
        //Ping
        StartCoroutine(StartPing(statusImg, server));

    }

    IEnumerator StartPing(Image img, Server server)
    {
        Ping pinger = new Ping(server.ipAddress);
        yield return new WaitForSeconds(1f);
        if (pinger.isDone)
        {
            img.color = Color.green;
            //Set IP if server responds
            m_IPv4AddIPField.text = server.ipAddress;
            
            //Set Type if server responds
            m_NetworkType.text = server.Type;
        }
        else
        {
            img.color = Color.red;
        }
    }

    public void SetUserName()
    {
        if (m_UsernameIPField.text != null)
        {
            ServerCheck.Instance.LocalClient.SetUserName(m_UsernameIPField.text);
            Debug.Log($"[UIManager]:: Set Local client username as: {m_UsernameIPField.text}");
        }
        else
        Debug.Log("Username null");

    }
    public void SetIPAddr()
    {
        if (m_IPv4AddIPField.text != null)
        {
            ServerCheck.Instance.SetServer(m_IPv4AddIPField.text);
            Debug.Log($"[UIManager]:: Set Server IP to Connect to: {m_IPv4AddIPField.text}");
        }
        else
            throw new Exception("[UI Manager]:: Need IP address of server for local client connect");

    }



    public void StartSession()
    {
        NetworkSystemControl.Singleton.StartGame();
    }



}
