using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XRSpatiotemopralAuthoring
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager _Instance;
        public static DataManager Instance { get { return _Instance; } }

        private DBNetworkingManager _dbNetworkingManager;
        [SerializeField] private Image _image;

        public List<ConstructionBuilding> _constructionBuildingComponents { private set; get; }

        public string dataFileCSV { private set; get; }

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        public void Start()
        {
            _dbNetworkingManager = DBNetworkingManager.Instance;
        }

        // Start is called before the first frame update
        public void StartConnecting()
        {
            try
            {
                _dbNetworkingManager.Connect();
                _constructionBuildingComponents = _dbNetworkingManager.RetrieveCollection("Construction");
                if (_constructionBuildingComponents != null)
                {
                    ConvertCSV(_constructionBuildingComponents);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            
        }


        public void ConvertCSV(List<ConstructionBuilding> list)
        {
            try
            {
                // Construct CSV string
                dataFileCSV = "Id,Name,Milestone,Size,Type,Material,Location,Height\n"; // Header
                foreach (ConstructionBuilding obj in list)
                {
                    dataFileCSV += $"{obj.id},{obj.name},{obj.milestone},{obj.size},{obj.type},{obj.material},{obj.location},{obj.height}\n"; // Data rows
                }
                Debug.Log("[DataManager]: Convert to CSV successful");
                _image.color = Color.green;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }



        }
        public string ConvertCSV()
        {
            try
            {
                // Construct CSV string
                dataFileCSV = "id,name,milestone,size,type,material,location,height\n"; // Header
                foreach (ConstructionBuilding obj in _constructionBuildingComponents)
                {
                    dataFileCSV += $"{obj.id},{obj.name},{obj.milestone},{obj.size},{obj.type},{obj.material},{obj.location},{obj.height}\n"; // Data rows
                }
                Debug.Log("[DataManager]: Convert to CSV successful");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return dataFileCSV;

        }
    }
    
 }

