using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Camera = UnityEngine.Camera;
using Image = UnityEngine.UI.Image;

public class ARObject : MonoBehaviour
{
    [Header("AR Object attributes")]
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private string type;
    [SerializeField] private string color;

    [Header("3D UI")]
    [SerializeField] private Transform UITransform;
    [SerializeField] private Button UndoButton;
    [SerializeField] private Image EditedImage;
    private Camera m_MainCamera;


    private Vector3 previousPosition;
    private Stack<Vector3> transformHistory;

    private void Start()
    {
        m_MainCamera = Camera.main;
        transformHistory = new Stack<Vector3>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        if (m_MainCamera != null)
        {
            UITransform.LookAt(2 * UITransform.position - m_MainCamera.transform.position);
        }

        if (transform.position != previousPosition)
        {
            RecordTransform();
            previousPosition = transform.position;
        }
    }

    public string GetMetaData()
    {
        return "Name: " + name + "\n" + "Description: " + description + "\n" + "Type: " + type + "\n" + "Color: " + color;
    }

    public void RecordTransform()
    {

        transformHistory.Push(transform.position);
        Debug.Log($"updating transform history new position: {transformHistory.Peek()}");
    }

    public void UndoTransform()
    {
        if (transformHistory.Count > 0)
        {
            Debug.Log($"undoing transform history new position: {transformHistory.Peek()}");
            transform.position = transformHistory.Pop();
        }
    }
}
