
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Server:MonoBehaviour
{
    [BsonId]
    [SerializeField] public ObjectId _id;
    [SerializeField] public string pcname;
    [SerializeField] public string Type;
    [SerializeField] public string ipaddress;
    [SerializeField] public string LastConnected;

    public void SetServerIP(string ipAddr)
    {
        this.ipaddress = ipAddr;
    }

    internal void SetServer(string pcName, string ipAddress, string type)
    {
        this.pcname = pcName;
        this.Type = type;
        this.ipaddress = ipAddress;
        this.LastConnected = DateTime.Now.ToString();
    }


}