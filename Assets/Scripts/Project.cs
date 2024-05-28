
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace XRSpatiotemopralAuthoring
{
    public class Project
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string ProjectId { private set; get; }
        public string ProjectName { private set; get; }
        public string Description { private set; get; }
        public string Stringify()
        {
            return JsonUtility.ToJson(this);
        }

        public static Project Parse(string json)
        {
            Debug.Log(json);
            return null;
        }
    }
}

