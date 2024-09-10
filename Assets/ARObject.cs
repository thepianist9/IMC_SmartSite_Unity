using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARObject : MonoBehaviour
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private string type;
    [SerializeField] private string color;


    public string GetMetaData()
    {
        return "Name: " + name + "\n" + "Description: " + description + "\n" + "Type: " + type + "\n" + "Color: " + color;
    }
}
