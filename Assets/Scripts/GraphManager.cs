using System;
using System.Collections;
using System.Collections.Generic;
using IATK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XRSpatiotemopralAuthoring
{
    public class GraphManager : MonoBehaviour
    {
        private DataManager dataInstance;
        public Pose m_GraphPose;

        private static GraphManager _Instance;
        [SerializeField] private TPPCameraSwitcher tPPCameraSwitcher;
        public static GraphManager Instance { get { return _Instance; } }

        [SerializeField] private TMP_Dropdown graphLabelNumberDropDown_3D;
        [SerializeField] private TMP_Dropdown graphLabelNumberDropDown_2D;
        [SerializeField] private GameObject graphDataLabeDropdownGOX_3D;
        [SerializeField] private GameObject graphDataLabeDropdownGOY_3D;
        [SerializeField] private GameObject graphDataLabeDropdownGOZ_3D;
        [SerializeField] private GameObject dataStatusOutline_3D;
        [SerializeField] private GameObject dataStatusOutline_2D;


        [SerializeField] private GameObject graphDataLabeDropdownGOX_2D;
        [SerializeField] private GameObject graphDataLabeDropdownGOY_2D;
        [SerializeField] private GameObject graphDataLabeDropdownGOZ_2D;
        [SerializeField] private Transform BoxTransform;

        


        [SerializeField] private GameObject SidePanelParentGO_2D;
        [SerializeField] private GameObject SidePanelParentGO_3D;
        [SerializeField] private GameObject SidePanelGO;



        //Graph Control Panel
        [SerializeField] private Transform CanvasTransform;
        [SerializeField] private GameObject GraphControlPanelPrefab_2D;
        [SerializeField] private GameObject GraphControlPanelPrefab_3D;
        [SerializeField] private TMP_Text Header;


        [SerializeField] private GameObject visualisationObject;

        public Dictionary<Toggle, Visualisation> visualisations = new Dictionary<Toggle, Visualisation>();
        //3D
        private TMP_Dropdown graphDataLabeDropdownX_3D;
        private TMP_Dropdown graphDataLabeDropdownY_3D;
        private TMP_Dropdown graphDataLabeDropdownZ_3D;
        //AR
        private TMP_Dropdown graphDataLabeDropdownX_2D;
        private TMP_Dropdown graphDataLabeDropdownY_2D;
        private TMP_Dropdown graphDataLabeDropdownZ_2D;
        internal GraphsControlManager currentGraphControlManager;

        private List<string> propertyNames = new List<string>();

        private Dictionary<Toggle, GameObject> togglePanelDictionary = new Dictionary<Toggle, GameObject>();

        public CSVDataSource myCSVDataSource;
        public bool isDataReady = false;

        private List<TMP_Dropdown> graphLabelDataDropdowns = new List<TMP_Dropdown>();
        private int graphAxisNumber = 0;
        private int graphNumber = 0;

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"GraphNumber: {graphNumber}");
      
            if (PlatformManager.Instance.platform == PlatformManager.Platform.AR )
            {
                //USE 2DUI
                graphDataLabeDropdownX_2D = graphDataLabeDropdownGOX_2D.GetComponentInChildren<TMP_Dropdown>();
                graphDataLabeDropdownY_2D = graphDataLabeDropdownGOY_2D.GetComponentInChildren<TMP_Dropdown>();
                graphDataLabeDropdownZ_2D = graphDataLabeDropdownGOZ_2D.GetComponentInChildren<TMP_Dropdown>();

                graphLabelDataDropdowns.Add(graphDataLabeDropdownX_2D);
                graphLabelDataDropdowns.Add(graphDataLabeDropdownY_2D);
                graphLabelDataDropdowns.Add(graphDataLabeDropdownZ_2D);
            }



            else if (PlatformManager.Instance.platform == PlatformManager.Platform.VR || PlatformManager.Instance.platform == PlatformManager.Platform.Editor)
            {
                //USE 3DUI
                graphDataLabeDropdownX_3D = graphDataLabeDropdownGOX_3D.GetComponentInChildren<TMP_Dropdown>();
                graphDataLabeDropdownY_3D = graphDataLabeDropdownGOY_3D.GetComponentInChildren<TMP_Dropdown>();
                graphDataLabeDropdownZ_3D = graphDataLabeDropdownGOZ_3D.GetComponentInChildren<TMP_Dropdown>();

                graphLabelDataDropdowns.Add(graphDataLabeDropdownX_3D);
                graphLabelDataDropdowns.Add(graphDataLabeDropdownY_3D);
                graphLabelDataDropdowns.Add(graphDataLabeDropdownZ_3D);

            }

            dataInstance = DataManager.Instance;
       

        }


        public void setGraphLabels()
        {
            if(dataInstance == null)
                dataInstance = DataManager.Instance;
            if (dataInstance._constructionBuildingComponents != null)
            {

                //convert and get csv data
                dataInstance.ConvertCSV(dataInstance._constructionBuildingComponents);
                // Get the type of ConstructionBuilding
                Type buildingType = typeof(ConstructionBuilding);

                // Get all properties of ConstructionBuilding
                buildingType.GetProperties().ForEach((prop => propertyNames.Add(prop.Name)));

                foreach (var dropDown in graphLabelDataDropdowns)
                {
                    dropDown.ClearOptions();
                    dropDown.AddOptions(propertyNames);
                }
                Debug.Log("[GraphManager]: Graph Labels Set...");
            }
        }

        public void SetDataSource()
        {
            if(gameObject.GetComponent<CSVDataSource>() == null)
            {
                myCSVDataSource = gameObject.AddComponent<CSVDataSource>();
                string result = dataInstance.ConvertCSV();
                myCSVDataSource.load(result, null);

                isDataReady = true;
                Debug.Log("[GraphManager]: Data source set for visualization");
                if(PlatformManager.Instance.platform == PlatformManager.Platform.Editor)
                {
                    dataStatusOutline_3D.GetComponent<Image>().color = Color.cyan;
                }
                if (PlatformManager.Instance.platform == PlatformManager.Platform.AR)
                {
                    dataStatusOutline_2D.GetComponent<Image>().color = Color.cyan;
                }
            }
            
        }

        public void SetGraphAxisNumber2D()
        {
            Debug.Log("set axis called");
            switch (graphLabelNumberDropDown_2D.options[graphLabelNumberDropDown_2D.value].text)
            {
                case "Single Axis":
                    graphDataLabeDropdownGOX_2D.SetActive(true);
                    graphDataLabeDropdownGOY_2D.SetActive(false);
                    graphDataLabeDropdownGOZ_2D.SetActive(false);
                    graphAxisNumber = 1;
                    break;
                case "Double Axis":
                    graphDataLabeDropdownGOX_2D.SetActive(true);
                    graphDataLabeDropdownGOY_2D.SetActive(true);
                    graphDataLabeDropdownGOZ_2D.SetActive(false);
                    graphAxisNumber = 2;
                    break;
                case "Triple Axis":
                    graphDataLabeDropdownGOX_2D.SetActive(true);
                    graphDataLabeDropdownGOY_2D.SetActive(true);
                    graphDataLabeDropdownGOZ_2D.SetActive(true);
                    graphAxisNumber = 3;
                    break;

            }
        }
        public void SetGraphAxisNumber3D()
        {
            Debug.Log("set axis called");
            switch (graphLabelNumberDropDown_3D.options[graphLabelNumberDropDown_3D.value].text)
            {
                case "Single Axis":
                    graphDataLabeDropdownGOX_3D.SetActive(true);
                    graphDataLabeDropdownGOY_3D.SetActive(false);
                    graphDataLabeDropdownGOZ_3D.SetActive(false);
                    graphAxisNumber = 1;
                    break;
                case "Double Axis":
                    graphDataLabeDropdownGOX_3D.SetActive(true);
                    graphDataLabeDropdownGOY_3D.SetActive(true);
                    graphDataLabeDropdownGOZ_3D.SetActive(false);
                    graphAxisNumber = 2;
                    break;
                case "Triple Axis":
                    graphDataLabeDropdownGOX_3D.SetActive(true);
                    graphDataLabeDropdownGOY_3D.SetActive(true);
                    graphDataLabeDropdownGOZ_3D.SetActive(true);
                    graphAxisNumber = 3;
                    break;

            }
        }

        public void SetGraph2D()
        {
            switch (graphLabelNumberDropDown_2D.options[graphLabelNumberDropDown_2D.value].text)
            {
                case "Single Axis":
                    if (graphDataLabeDropdownX_2D.value != 0 || graphDataLabeDropdownY_2D.value != 0 || graphDataLabeDropdownZ_2D.value != 0)
                    {
                        //test2();
                        //create single axis graph
                        LoadGraph("Single Axis");
                    }
                    break;
                case "Double Axis":
                    if ((graphDataLabeDropdownX_2D.value != 0 && graphDataLabeDropdownY_2D.value != 0) || (graphDataLabeDropdownX_2D.value != 0 && graphDataLabeDropdownZ_2D.value != 0) || (graphDataLabeDropdownY_2D.value != 0 && graphDataLabeDropdownZ_2D.value != 0))
                    {
                        //test2();
                        //create double axis graph
                        LoadGraph("Double Axis");
                    }
                    break;
                case "Triple Axis":
                    if (graphDataLabeDropdownX_2D.value != 0 && graphDataLabeDropdownY_2D.value != 0 && graphDataLabeDropdownZ_2D.value != 0)
                    {
                        //test2();
                        //create single axis graph
                        LoadGraph("Triple Axis");
                    }
                    break;
                default:
                    Debug.Log($"[Graph Manager]: not all graphs axis selected for the graph with axis number: {graphAxisNumber}");
                    break;

            }
        }
        public void SetGraph3D()
        {
            switch (graphLabelNumberDropDown_3D.options[graphLabelNumberDropDown_3D.value].text)
            {
                case "Single Axis":
                    if (graphDataLabeDropdownX_3D.value != 0 || graphDataLabeDropdownY_3D.value != 0 || graphDataLabeDropdownZ_3D.value != 0)
                    {
                        //test2();
                        //create single axis graph
                        LoadGraph("Single Axis");
                    }
                    break;
                case "Double Axis":
                    if ((graphDataLabeDropdownX_3D.value != 0 && graphDataLabeDropdownY_3D.value != 0) || (graphDataLabeDropdownX_3D.value != 0 && graphDataLabeDropdownZ_3D.value != 0) || (graphDataLabeDropdownY_3D.value != 0 && graphDataLabeDropdownZ_3D.value != 0))
                    {
                        //test2();
                        //create double axis graph
                        LoadGraph("Double Axis");
                    }
                    break;
                case "Triple Axis":
                    if (graphDataLabeDropdownX_3D.value != 0 && graphDataLabeDropdownY_3D.value != 0 && graphDataLabeDropdownZ_3D.value != 0)
                    {
                        //test2();
                        //create single axis graph
                        LoadGraph("Triple Axis");
                    }
                    break;
                default:
                    Debug.Log($"[Graph Manager]: not all graphs axis selected for the graph with axis number: {graphAxisNumber}");
                    break;

            }

        }






        public void LoadGraph(string NoOfAxis)
        {

            if(isDataReady)
            {
                

                CSVDataSource csv = GetComponent<CSVDataSource>();
                GameObject go = null;
                GameObject toggleGO = null;
               
                //go = GameObject.Instantiate(visualisationObject, m_GraphPosition + Vector3.forward * (graphNumber - 1), Quaternion.AngleAxis(90f, Vector3.up));
                if (PlatformManager.Instance.platform == PlatformManager.Platform.AR )
                {

                    go = Instantiate(visualisationObject, m_GraphPose.position + Vector3.forward * (graphNumber - 1), m_GraphPose.rotation);
                }
                else if(PlatformManager.Instance.platform == PlatformManager.Platform.Editor || PlatformManager.Instance.platform == PlatformManager.Platform.VR)
                {
                    go = Instantiate(visualisationObject, BoxTransform.position + Vector3.up * 1 + Vector3.forward * (graphNumber - 1), BoxTransform.rotation);
                }

                Visualisation v = go.GetComponent<Visualisation>();
                v.name = $"Graph: {graphNumber}";
                v.dataSource = csv;
                v.geometry = AbstractVisualisation.GeometryType.Points;

                if ( PlatformManager.Instance.platform == PlatformManager.Platform.AR)
                {
                    switch (NoOfAxis)
                    {
                        case "Single Axis":
                            if (graphDataLabeDropdownX_2D.value != 0)
                            {
                                //create single axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_2D.options[graphDataLabeDropdownX_2D.value].text;


                                //vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X);
                            }
                            break;
                        case "Double Axis":
                            if (graphDataLabeDropdownX_2D.value != 0 && graphDataLabeDropdownY_2D.value != 0)
                            {
                                //create double axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_2D.options[graphDataLabeDropdownX_2D.value].text;
                                v.yDimension.Attribute = graphDataLabeDropdownY_2D.options[graphDataLabeDropdownY_2D.value].text;
                                /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                     setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y);*
     */

                            }
                            break;
                        case "Triple Axis":
                            if (graphDataLabeDropdownX_2D.value != 0 && graphDataLabeDropdownY_2D.value != 0 && graphDataLabeDropdownZ_2D.value != 0)
                            {
                                //create Triple axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_2D.options[graphDataLabeDropdownX_2D.value].text;
                                v.yDimension.Attribute = graphDataLabeDropdownY_2D.options[graphDataLabeDropdownY_2D.value].text;
                                v.zDimension.Attribute = graphDataLabeDropdownZ_2D.options[graphDataLabeDropdownZ_2D.value].text;
                                /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                     setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y).
                                     setDataDimension(csv[graphDataLabeDropdownZ.options[graphDataLabeDropdownZ.value].text].Data, ViewBuilder.VIEW_DIMENSION.Z);*/
                            }
                            break;

                        default:
                            Debug.Log($"[Graph Manager]: Load Graph failed");
                            break;
                    }

                    //set in 2D graph panel
                    toggleGO = Instantiate(SidePanelGO, SidePanelParentGO_2D.transform);
                }
                else if (PlatformManager.Instance.platform == PlatformManager.Platform.VR|| PlatformManager.Instance.platform == PlatformManager.Platform.Editor)
                {
                    switch (NoOfAxis)
                    {
                        case "Single Axis":
                            if (graphDataLabeDropdownX_3D.value != 0)
                            {
                                //create single axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_3D.options[graphDataLabeDropdownX_3D.value].text;


                                //vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X);
                            }
                            break;
                        case "Double Axis":
                            if (graphDataLabeDropdownX_3D.value != 0 && graphDataLabeDropdownY_3D.value != 0)
                            {
                                //create double axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_3D.options[graphDataLabeDropdownX_3D.value].text;
                                v.yDimension.Attribute = graphDataLabeDropdownY_3D.options[graphDataLabeDropdownY_3D.value].text;
                                /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                     setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y);*
     */

                            }
                            break;
                        case "Triple Axis":
                            if (graphDataLabeDropdownX_3D.value != 0 && graphDataLabeDropdownY_3D.value != 0 && graphDataLabeDropdownZ_3D.value != 0)
                            {
                                //create Triple axis graph
                                v.xDimension.Attribute = graphDataLabeDropdownX_3D.options[graphDataLabeDropdownX_3D.value].text;
                                v.yDimension.Attribute = graphDataLabeDropdownY_3D.options[graphDataLabeDropdownY_3D.value].text;
                                v.zDimension.Attribute = graphDataLabeDropdownZ_3D.options[graphDataLabeDropdownZ_3D.value].text;
                                /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                     setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y).
                                     setDataDimension(csv[graphDataLabeDropdownZ.options[graphDataLabeDropdownZ.value].text].Data, ViewBuilder.VIEW_DIMENSION.Z);*/
                            }
                            break;

                        default:
                            Debug.Log($"[Graph Manager]: Load Graph failed");
                            break;
                    }

                    //set in 3D graph panel
                    toggleGO = Instantiate(SidePanelGO, SidePanelParentGO_3D.transform);
                    
                }
                /*//Enumerable.Repeat(1f, dataSource[0].Data.Length).ToArray()
                Material mt = new Material(Shader.Find("IATK/OutlineDots"));
                //Material mt = new Material(Shader.Find("IATK/LinesShader"));
                mt.mainTexture = Resources.Load("circle-outline-basic") as Texture2D;
                mt.renderQueue = 3000;
                mt.SetFloat("_MinSize", 0.01f);
                mt.SetFloat("_MaxSize", 0.05f);


                // Create a view builder with the point topology
                View view = vb.updateView().apply(gameObject, mt);*/

                v.sizeDimension = "Undefined";
                v.colourDimension = "Undefined";
                v.colorPaletteDimension = "Undefined";
                v.colour = Color.red;

                v.size = 0.5f;
                //v.minSize = 0.1f;
                //v.maxSize = 0.3f;
               
                
                
                graphNumber += 1;
                if(toggleGO != null)
                    toggleGO.GetComponentInChildren<TMP_Text>().text = $"Graph: {graphNumber}";


                Toggle toggle = toggleGO.GetComponentInChildren<Toggle>();
                toggle.isOn = true;

                if (PlatformManager.Instance.platform == PlatformManager.Platform.VR || PlatformManager.Instance.platform == PlatformManager.Platform.Editor)
                {
                    toggle.group = SidePanelParentGO_3D.GetComponent<ToggleGroup>();
                    currentGraphControlManager = GraphControlPanelPrefab_3D.GetComponent<GraphsControlManager>();
                    currentGraphControlManager.visualisation = v;

                    UIManager.Instance.SwitchContextMenu();
                }  
                


                if (PlatformManager.Instance.platform == PlatformManager.Platform.AR)
                {
                    toggle.group = SidePanelParentGO_2D.GetComponent<ToggleGroup>();
                    currentGraphControlManager = GraphControlPanelPrefab_2D.GetComponent<GraphsControlManager>();
                    currentGraphControlManager.visualisation = v;
                }
                    

                toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
                toggle.gameObject.name = $"Graph: {graphNumber}";


                //instantiate and then on toggle activate the corresponding go
                


                //pair of toggles and panels
                //togglePanelDictionary.Add(toggle, GraphControl);
                

                //visualisations.Add(toggle, v);



                v.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
                v.updateProperties();

                


                //create panel to manage graphs
                //add created graph to the graph panel


            }
        }

        private void ToggleValueChanged(Toggle toggle)
        {
            // Get the index of the toggled toggle
            
            togglePanelDictionary[toggle].SetActive(toggle.isOn);



            // Check if the index is valid and the corresponding GameObject exists
            
            if (toggle.isOn)
            {
                
                Header.text = toggle.gameObject.name;
            }
        }

    }  
}

