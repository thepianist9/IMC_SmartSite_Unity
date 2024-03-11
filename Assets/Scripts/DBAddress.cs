using UnityEngine;


namespace XRSpatiotemopralAuthoring
{
    [CreateAssetMenu]
    public class DBAddress : ScriptableObject
    {
        public string DBName = "mongodb";
        public string DBIPAddress = "127.0.0.1";
        public string DBport = "27017";
        
        //API Token
        
        //paths
        //construction
        public string construtionPath = "Construction";
        public string BuildingAPath = "BuildingA";
        //Poject B,C,D...



    }
}
