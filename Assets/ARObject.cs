
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;

using UnityEngine.UI;
using Camera = UnityEngine.Camera;
using Image = UnityEngine.UI.Image;

public class ARObject : NetworkBehaviour
{
    [Header("AR Object attributes")]
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private string type;
    [SerializeField] public string color;
    [SerializeField] public MeshRenderer meshRenderer;

    [Header("3D UI")]
    [SerializeField] private Transform UITransform;
    [SerializeField] private Button UndoButton;
    [SerializeField] private Image EditedImage;
    private Camera m_MainCamera;


    private Vector3 previousPosition;
    private Stack<Vector3> transformHistory;
    [SerializeField] public NetworkVariable<Color> m_ObjectColor = new NetworkVariable<Color>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        m_ObjectColor.OnValueChanged += OnColorChanged;
    }
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

    //Network Variable section
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Assin the current value based on the current message index value
            m_ObjectColor.Value = GetComponent<MeshRenderer>().material.color;
            color = m_ObjectColor.Value.ToString();
        }
        else
        {
            // Subscribe to the OnValueChanged event
            m_ObjectColor.OnValueChanged += OnColorChanged;
            color = m_ObjectColor.Value.ToString();
            // Log the current value of the text string when the client connected
            Debug.Log($"Client-{NetworkManager.LocalClientId}'s TextString = {m_ObjectColor.Value}");
        }
    }

    public override void OnNetworkDespawn()
    {
        m_ObjectColor.OnValueChanged -= OnColorChanged;
    }

    private void OnColorChanged(Color previousValue, Color newValue)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previousValue} | Current: {newValue}");

        //change color of the object
        color = m_ObjectColor.Value.ToString();

        ChangeMaterials(newValue);
    }

    private void ChangeMaterials(Color newValue)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        Material material = new Material(meshRenderer.materials[0]);
        material.color = newValue;
        if(meshRenderer.materials.Length > 1)
        {
            meshRenderer.materials = new Material[] { meshRenderer.materials[0] };
        }
        else
        {
            meshRenderer.AddMaterial(material);
        }
    }
}
