using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IATK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace XRSpatiotemopralAuthoring
{
    public class GraphManager : MonoBehaviour
    {
        private DataManager dataInstance;

        private static GraphManager _Instance;
        public static GraphManager Instance { get { return _Instance; } }

        [SerializeField] private TMP_Dropdown graphLabelNumberDropDown;
        [SerializeField] private GameObject graphDataLabeDropdownGOX;
        [SerializeField] private GameObject graphDataLabeDropdownGOY;
        [SerializeField] private GameObject graphDataLabeDropdownGOZ;
        [SerializeField] private Transform BoxTransform;

        


        [SerializeField] private GameObject GraphPanel;
        [SerializeField] private GameObject SidePanelParentGO;
        [SerializeField] private GameObject SidePanelGO;


        //Graph Control Panel
        [SerializeField] private Transform CanvasTransform;
        [SerializeField] private GameObject GraphControlPanelPrefab;
        [SerializeField] private TMP_Text Header;


        [SerializeField] private GameObject visualisationObject;

        public Dictionary<Toggle, Visualisation> visualisations = new Dictionary<Toggle, Visualisation>();

        private TMP_Dropdown graphDataLabeDropdownX;
        private TMP_Dropdown graphDataLabeDropdownY;
        private TMP_Dropdown graphDataLabeDropdownZ;

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
            graphDataLabeDropdownX = graphDataLabeDropdownGOX.GetComponentInChildren<TMP_Dropdown>();
            graphDataLabeDropdownY = graphDataLabeDropdownGOY.GetComponentInChildren<TMP_Dropdown>();
            graphDataLabeDropdownZ = graphDataLabeDropdownGOZ.GetComponentInChildren<TMP_Dropdown>();

            dataInstance = DataManager.Instance;

            graphLabelDataDropdowns.Add(graphDataLabeDropdownX);
            graphLabelDataDropdowns.Add(graphDataLabeDropdownY);
            graphLabelDataDropdowns.Add(graphDataLabeDropdownZ);

            graphLabelNumberDropDown.onValueChanged.AddListener(delegate { { SetGraphAxisNumber(); } });
        }


        public void setGraphLabels()
        {
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
            }
            
        }

        private void SetGraphAxisNumber()
        {
            Debug.Log("set axis called");
            switch (graphLabelNumberDropDown.options[graphLabelNumberDropDown.value].text)
            {
                case "Single Axis":
                    graphDataLabeDropdownGOX.SetActive(true);
                    graphDataLabeDropdownGOY.SetActive(false);
                    graphDataLabeDropdownGOZ.SetActive(false);
                    graphAxisNumber = 1;
                    break;
                case "Double Axis":
                    graphDataLabeDropdownGOX.SetActive(true);
                    graphDataLabeDropdownGOY.SetActive(true);
                    graphDataLabeDropdownGOZ.SetActive(false);
                    graphAxisNumber = 2;
                    break;
                case "Triple Axis":
                    graphDataLabeDropdownGOX.SetActive(true);
                    graphDataLabeDropdownGOY.SetActive(true);
                    graphDataLabeDropdownGOZ.SetActive(true);
                    graphAxisNumber = 3;
                    break;

            }
        }

        public void SetGraph()
        {
            switch (graphLabelNumberDropDown.options[graphLabelNumberDropDown.value].text)
            {
                case "Single Axis":
                    if (graphDataLabeDropdownX.value != 0 || graphDataLabeDropdownY.value != 0 || graphDataLabeDropdownZ.value != 0)
                    {
                        //test2();
                        //create single axis graph
                        LoadGraph("Single Axis");
                    }
                    break;
                case "Double Axis":
                    if ((graphDataLabeDropdownX.value != 0 && graphDataLabeDropdownY.value != 0) || (graphDataLabeDropdownX.value != 0 && graphDataLabeDropdownZ.value != 0) || (graphDataLabeDropdownY.value != 0 && graphDataLabeDropdownZ.value != 0))
                    {
                        //test2();
                        //create double axis graph
                        LoadGraph("Double Axis");
                    }
                    break;
                case "Triple Axis":
                    if (graphDataLabeDropdownX.value != 0 && graphDataLabeDropdownY.value != 0 && graphDataLabeDropdownZ.value != 0)
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
                GameObject go = GameObject.Instantiate(visualisationObject, BoxTransform.position + Vector3.up * 1 + Vector3.forward * (graphNumber-1), Quaternion.AngleAxis(90f, Vector3.up));
                Visualisation v = go.GetComponent<Visualisation>();
                v.name = $"Graph: {graphNumber}";
                v.dataSource = csv;
                v.geometry = AbstractVisualisation.GeometryType.Points;

                switch (NoOfAxis)
                {
                    case "Single Axis":
                        if (graphDataLabeDropdownX.value != 0)
                        {
                            //create single axis graph
                            v.xDimension.Attribute = graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text;
                            

                            //vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X);
                        }
                        break;
                    case "Double Axis":
                        if (graphDataLabeDropdownX.value != 0 && graphDataLabeDropdownY.value != 0)
                        {
                            //create double axis graph
                            v.xDimension.Attribute = graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text;
                            v.yDimension.Attribute = graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text;
                            /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                 setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y);*
 */

                        }
                        break;
                    case "Triple Axis":
                        if (graphDataLabeDropdownX.value != 0 && graphDataLabeDropdownY.value != 0 && graphDataLabeDropdownZ.value != 0)
                        {
                            //create Triple axis graph
                            v.xDimension.Attribute = graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text;
                            v.yDimension.Attribute = graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text;
                            v.zDimension.Attribute = graphDataLabeDropdownZ.options[graphDataLabeDropdownZ.value].text;
                            /* vb.setDataDimension(csv[graphDataLabeDropdownX.options[graphDataLabeDropdownX.value].text].Data, ViewBuilder.VIEW_DIMENSION.X).
                                 setDataDimension(csv[graphDataLabeDropdownY.options[graphDataLabeDropdownY.value].text].Data, ViewBuilder.VIEW_DIMENSION.Y).
                                 setDataDimension(csv[graphDataLabeDropdownZ.options[graphDataLabeDropdownZ.value].text].Data, ViewBuilder.VIEW_DIMENSION.Z);*/
                        }
                        break;

                    default:
                        Debug.Log($"[Graph Manager]: Load Graph failed");
                        break;
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
                if (!GraphPanel.gameObject.activeSelf)
                    GraphPanel.gameObject.SetActive(true);
                GameObject toggleGO = Instantiate(SidePanelGO, SidePanelParentGO.transform);
                graphNumber += 1;
                toggleGO.GetComponentInChildren<TMP_Text>().text = $"Graph: {graphNumber}";

                Toggle toggle = toggleGO.GetComponentInChildren<Toggle>();
                toggle.isOn = true;
                toggle.group = SidePanelParentGO.GetComponent<ToggleGroup>();
                toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
                toggle.gameObject.name = $"Graph: {graphNumber}";


                //instantiate and then on toggle activate the corresponding go
                GraphControlPanelPrefab.GetComponent<GraphsControlManager>().visualisation = v;


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

