using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;
using UnityEngine.Networking;

namespace XRSpatiotemopralAuthoring
{
    public class DBNetworkingManager : MonoBehaviour
    {
        public static DBNetworkingManager Instance { get; private set; }
        private const int maxRetry = 3;
        public DBAddress dbAddress;
        private string dBConnectionString = String.Empty;

        private MongoClient _mongoClient;
        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this);
            } 
            else 
            { 
                Instance = this; 
            } 
            
            //Setup <mongodb>://<ipadd>:<portnumber>
            dBConnectionString = dbAddress.DBName + "://" + dbAddress.DBIPAddress + ":" + dbAddress.DBport;
        }

        public void Connect()
        {
            _mongoClient = new MongoClient(dBConnectionString);
            Debug.Log($"Connected to: {dBConnectionString}");
            
        }
        public List<Project> RetrieveProjectCollection(string collectionName)
        {
            var database = _mongoClient.GetDatabase("local");
            var constructionCollection = database.GetCollection<Project>(collectionName);
            var documents = constructionCollection.Find(_ => true).ToList(); // Retrieve all documents in the collection
            Debug.Log($"documents fetched from {collectionName}");

            return documents;
        }

        public List<ConstructionBuilding> RetrieveCollection(string collectionName)
        {
            var database = _mongoClient.GetDatabase("local");
            var constructionCollection = database.GetCollection<ConstructionBuilding>(collectionName);
            var documents = constructionCollection.Find(_ => true).ToList(); // Retrieve all documents in the collection
            Debug.Log($"documents fetched from {collectionName}");

            return documents;
        }
        
    }
}