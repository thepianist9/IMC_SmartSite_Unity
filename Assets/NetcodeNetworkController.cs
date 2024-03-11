using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetcodeNetworkController : NetworkBehaviour
{
    [SerializeField] private GameObject DynamicNetcodeObject;
    [SerializeField] private GameObject OfflineClientGO;
    [SerializeField] private NetworkManager networkManagerGO;

    private NetworkManager networkManager;
    private UnityTransport utp;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = networkManagerGO.GetComponent<NetworkManager>();
        utp = networkManagerGO.GetComponent<UnityTransport>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkObject networkObject = DynamicNetcodeObject.AddComponent<NetworkObject>();
                DynamicNetcodeObject.AddComponent<NetworkTransform>();
                networkObject.Spawn();
            }
            
        }
        
    }

    public void StartHost()
    {
        if(utp != null)
        {
            RegisterNetworkPrefab(OfflineClientGO);
            NetworkManager.Singleton.StartHost();
        }
    }
    public void StartClient()
    {
        if (utp != null)
        {
            RegisterNetworkPrefab(OfflineClientGO);
            NetworkManager.Singleton.StartHost();
        }
    }
    public override void OnNetworkSpawn()
    {
       
        OfflineClientGO.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);

    }

    private GameObject SetGONetcodeReady(GameObject  go)
    {
        go.AddComponent<NetworkObject>();
        go.AddComponent<NetworkTransform>();
        return go;
    }

    private void RegisterNetworkPrefab(GameObject prefab)
    {
        GameObject go = SetGONetcodeReady(prefab);
        NetworkManager.Singleton.AddNetworkPrefab(go);
    }
}
