using System.IO;
using System.Net;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using MongoDB.Driver;
using System.Collections;


public class ServerCheck : MonoBehaviour
{
    public string m_PcName;
    public string m_ClientIP;
    public string m_ServerType;

    public bool IsServer = false;
    public List<Server> serverList = new List<Server>();


    //GameClient Server Variables
    public Client LocalClient;
    public Server server;


    private MongoClient _mongoClient;
    [SerializeField] private string dBConnectionString = "mongodb://192.168.188.21:27017";
    [SerializeField] private bool dbLocal = true;
            
    public static ServerCheck Instance;

    private void Awake() 
    {
        if (Instance != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(ServerCheck)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
            
        }
        Instance = this;

        


        m_PcName = Dns.GetHostName();
        m_ClientIP = Dns.GetHostEntry(m_PcName).AddressList[1].ToString(); //[1]: office, [3]: home



        SceneManager.sceneLoaded += ReadServerInfo;

        LocalClient = new Client(m_PcName, m_ClientIP);

    }

    private void ConnectMongoDBServer()
    {
        _mongoClient = new MongoClient(dBConnectionString);

    }

    private void RetrieveServerList()
    {
        if (dbLocal)
        {
            var database = _mongoClient.GetDatabase("local");
            var constructionCollection = database.GetCollection<Server>("Servers");
            var documents = constructionCollection.Find(_ => true).ToList(); // Retrieve all documents in the collection
            
            foreach(var document in documents)
            {
                serverList.Add(document);
            }
        }
        else
        {
            Debug.Log("Online DB not set");
        }
        
    }

    private void ReadServerInfo(Scene scene, LoadSceneMode arg1)
    {
        Debug.Log($"[Scene {scene.name}]: LOADED");
        if(string.Equals(scene.name, "StartMenu"))
        {
            ReadServerInfo();
        }
    }

    private void ReadServerInfo()
    {
        
        if (NetworkSystemControl.Singleton.IsOnline)
        {
            ConnectMongoDBServer();
            RetrieveServerList();

            foreach (var server in serverList)
            {
                Debug.Log(server.ipaddress);

                if (string.Equals(m_ClientIP, server.ipaddress, StringComparison.OrdinalIgnoreCase))
                {
                    m_ServerType = server.Type;

                    IsServer = true;
                    SetServer(server.ipaddress);
                    //Start server directly on machine:
                    //NetworkSystemControl.Singleton.StartSessionOnline();

                    UIManager.Instance.DisplayServerMenu(server);



                    return;
                }


            }
            if(serverList.Count > 0) {
                UIManager.Instance.DisplayClientMenu(serverList);
            }
            else
            {
            }
            
            IsServer = false;
            return;
        }
        else
        {
            Debug.Log("[ServerCheck]: No system check entering Offline mode");
        }
        


    }

    public void SetServer(string IP)
    {
        if(server == null)
        {
            server = new Server();
            server.SetServerIP(IP);
        }

        if (!IsServer)
        {
            server.SetServerIP(IP);
            Debug.Log("Remote Server Sett!");
        }

    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= ReadServerInfo;
    }



}
