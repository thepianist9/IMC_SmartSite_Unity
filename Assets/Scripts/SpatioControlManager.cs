using IATK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using UnityEngine;
using XRSpatiotemopralAuthoring;
using MongoDB.Driver;
using UnityEditor;
using TMPro;

public class SpatioControlManager : MonoBehaviour
{
    private static SpatioControlManager _Instance;
    public static SpatioControlManager Instance { get { return _Instance; } }
    [SerializeField] private TMP_Text _DescriptionPanelText;



    //take the data of the current visualisation object and get all the game objects
    //function to set higlight material of the retrieved game objects
    // function to activate only the game objects are filtered
    //function to create a link between data source and game object instances.
    private GraphManager _graphManager;
    private GraphsControlManager _graphsControlManagerInstance;
    [SerializeField] private GameObject _constructionObject;
    private DataSource _dataSource;
    private float[] dict;

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
        _constructionObject = GameObject.FindGameObjectWithTag("AR_Construction");

        if (_graphsControlManagerInstance == null)
        {
            //holds graph data
            _graphsControlManagerInstance = GraphsControlManager.Instance;
        }
        if (_graphManager == null)
        {
            //holds data
            _graphManager = GraphManager.Instance;
        }
        _dataSource = _graphManager.myCSVDataSource;
    }

    public void SetData()
    {
        //we get the filtered attribute
        //we get the filter min max values_
        _constructionObject = GameObject.FindGameObjectWithTag("AR_Construction");
        //
        AttributeFilter attributeFilter = _graphsControlManagerInstance.visualisation.attributeFilters[0];

        float min = attributeFilter.minFilter;
        float max = attributeFilter.maxFilter;



        var filteredItems = _dataSource.Select(item =>
        {

            if (item.Identifier == attributeFilter.Attribute)
            {
                dict = item.Data;
            } // Assuming item is the type you're filtering and it has a property called value to filter // Change this according to your actual structure


            // Check if the value falls within the range specified by min and max
            return dict;
        }).ToList();


        var indices = dict
                            .Select((value, index) => new { Value = value, Index = index }) // Project each value with its index
                            .Where(item => item.Value >= min && item.Value <= max) // Filter items within the range
                            .Select(item => item.Index);

        List<String> filteredgos = new List<string>();

        foreach (var index in indices)
        {
            Debug.Log($"Index: {index}, Value: {dict[index]}");
            var data = _dataSource
                            .Where(item => item.Identifier == "name")
                            .Select(item =>
                            {
                                return _dataSource.getOriginalValue(item.Data[index], "name");
                            })
                            .ToList();

            filteredgos.Add(data[0].ToString());
        }


        for (int i = 0; i < _constructionObject.transform.childCount; i++)
        {
            GameObject childGO = _constructionObject.transform.GetChild(i).gameObject;
            if (filteredgos.Contains(childGO.name))
                childGO.SetActive(true);
            else
                childGO.SetActive(false);


        }








        //get the attribute
        //get the data from datasource with that attribut

        //Debug.Log(columnData.ToString());
        //get the values in between the filter 
        //get their indices from original csv data source 
        //get names of game objects

    }
    public void GetDataFromID(string name)
    {
        //we get the filtered attribute
        //we get the filter min max values_
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Debug.Log(DataManager.Instance._constructionBuildingComponents);
        
        foreach(var constructionObject in DataManager.Instance._constructionBuildingComponents)
        {
            if(constructionObject.name == name)
            {
                dict.Add("Name", constructionObject.name);
                dict.Add("Milestone", constructionObject.milestone);
                dict.Add("Size", constructionObject.size.ToString());
                dict.Add("Type", constructionObject.type);
                dict.Add("Material", constructionObject.material);
                dict.Add("Location", constructionObject.location);
                dict.Add("Height", constructionObject.height.ToString());
            }
        }   
        _DescriptionPanelText.text = "";
        foreach(var item in dict)
        {
            _DescriptionPanelText.text += item.Key + " : " + item.Value + "\n";
        }



        /*  var indices = _dataSource
                .Select((value, index) => new { Value = value, Index = index }) // Project each value with its index
                .Where(item => item.Value.Equals(name, StringComparison.OrdinalIgnoreCase)) // Filter items within the range
    *//*            .Select(item => item.Index);*//*

            List<String> filteredgos = new List<string>();

            foreach (var index in indices)
            {
                Debug.Log($"Index: {index}, Value: {dict[index]}");
                var data = _dataSource
                            .Where(item => item.Identifier == "name")
                            .Select(item =>
                            {
                                return _dataSource.getOriginalValue(item.Data[index], "name");
                            })
                            .ToList();

                filteredgos.Add(data[0].ToString());
            }

            Debug.Log(filteredgos[0]);*/


    }
}
