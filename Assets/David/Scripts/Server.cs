
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Server:MonoBehaviour
{
    [SerializeField] public string pcName;
    [SerializeField] public string Type;
    [SerializeField] public string ipAddress;
    [SerializeField] public int clientID;

    public void SetServerIP(string ipAddr)
    {
        this.ipAddress = ipAddr;
    }

    internal void SetServer(string pcName, string ipAddress, string type)
    {
        this.pcName = pcName;
        this.Type = type;
        this.ipAddress = ipAddress;
    }


}