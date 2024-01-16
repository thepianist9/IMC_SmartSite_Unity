using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// A script to set the color of each player based on OwnerClientId
/// </summary>

public class Client 
    {
        [SerializeField] public string userName;
        [SerializeField] public string pcName;
        [SerializeField] public string ipAddr;
        [SerializeField] public Color clientColor;
        [SerializeField] public string timeConnected;
        private string clientIP;

        public Client(string pcName, string clientIP)
        {
            this.pcName = pcName;
            this.clientIP = clientIP;

        }

        public Client(string userName, string pcName, string ipAddr, Color clientColor)
        {
            this.userName = userName;
            this.pcName = pcName;
            this.ipAddr = ipAddr;
            this.clientColor = clientColor;
        }


        public void SetUserName(string userName)
        {
            this.userName = userName;
            this.timeConnected = DateTime.UtcNow.ToString("HH:mm");
        }

    }
