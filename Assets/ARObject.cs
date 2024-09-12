using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARObject : MonoBehaviour
{
    [Header("AR Object attributes")]
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private string type;
    [SerializeField] private string color;

    [Header("3D UI")]
    [SerializeField] private Transform UITransform;
    private Camera m_MainCamera;


    private void Start()
    {
        m_MainCamera = Camera.main;
    }
    public string GetMetaData()
    {
        return "Name: " + name + "\n" + "Description: " + description + "\n" + "Type: " + type + "\n" + "Color: " + color;
    }

    private void Update()
    {
        if (m_MainCamera != null)
        {
            UITransform.LookAt(2 * UITransform.position - m_MainCamera.transform.position);
        }
    }
}
