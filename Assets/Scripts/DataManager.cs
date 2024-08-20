using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XRSpatiotemopralAuthoring
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager _Instance;
        public static DataManager Instance { get { return _Instance; } }

        private DBNetworkingManager _dbNetworkingManager;
        [SerializeField] private Image _UIBorder;
        [SerializeField] private Image _UIBorderAR;
        [SerializeField] private TMP_Dropdown _projectDropdown;

        [SerializeField] private List<GameObject> _projectModels = new List<GameObject>();
        private List<Project> _projectList;
        private List<string> _projectNames;
        public List<ConstructionBuilding> _constructionBuildingComponents { private set; get; }

        public string dataFileCSV { private set; get; }

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        public void Start()
        {
            _dbNetworkingManager = DBNetworkingManager.Instance;
            _projectNames = new List<string>();
        }

        // Start is called before the first frame update
        public void StartConnecting()
        {
            try
            {
                _dbNetworkingManager.Connect();
                //Get Project List
                _projectList = _dbNetworkingManager.RetrieveProjectCollection("Projects");
                if (_projectList != null)
                {
                    Debug.Log("[DataManager]: Retrieve project list successful");
                    _projectNames.Add("Select Project");

                    foreach (Project project in _projectList)
                    {
                        _projectNames.Add(project.ProjectName);
                    }
                    _projectDropdown.AddOptions(_projectNames);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            
        }



        public void RetrieveProjectData()
        {
            try
            {
                //string projectName = _projectDropdown.options[_projectDropdown.value].text;

                _constructionBuildingComponents = _dbNetworkingManager.RetrieveCollection("Simple Building");
                if (_constructionBuildingComponents != null)
                {
                    ConvertCSV(_constructionBuildingComponents);
                    Debug.Log("[DataManager]: Retrieve project data successful");
                    //Load Corresponding 3d Model
                    LoadConstructionModel("Simple Building");
                }
                else
                {
                    Debug.Log("[DataManager]: Retrieve project data failed");
                    _UIBorder.color = Color.red;
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
                //2d<0>3d<1>
                _UIBorder.color = Color.green;
                
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                _UIBorder.color = Color.red;
            }



        }
        public string ConvertCSV()
        {
            if(_constructionBuildingComponents == null)
            {
                _constructionBuildingComponents = _dbNetworkingManager.RetrieveCollection("Simple Building");
            }
           

            
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
        public void LoadConstructionModel(string projectName)
        {

            //Load 3d Model
            //2d<0>3d<1>
            foreach (GameObject model in _projectModels)
            {
                if (model.name == projectName)
                {
                    model.SetActive(true);
                }
                else
                {
                    model.SetActive(false);
                }
            }
            Debug.Log("[DataManager]: Load 3d Model successful");
        }
            
        }
}

