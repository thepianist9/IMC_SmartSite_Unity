using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace  XRSpatiotemopralAuthoring
{
    public class ConstructionBuilding
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { private set; get; }
        public string name { private set; get; }
        
        public string milestone { private set; get; }
        public int size { private set; get; }
        public string type { private set; get; }
        public string material { private set; get; }
        public string location { private set; get; }
        public double height { private set; get; }
        
        public string Stringify() 
        {
            return JsonUtility.ToJson(this);
        }

        public static ConstructionBuilding Parse(string json)
        {
            Debug.Log(json);
            return null;
        }
    }
}

