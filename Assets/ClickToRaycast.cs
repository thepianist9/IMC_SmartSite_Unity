using Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class ClickToRaycast : MonoBehaviour
{
    [SerializeField] private GameObject MetaDataPanel;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private AppController m_AppController;

    [SerializeField] private Material SelectedMaterial;
    [SerializeField] private Material ColorChangeMaterial;
    [SerializeField] private GameObject m_MessageBox;
     
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "ARSpawnable")
                {
                    //toggle UI for gameobject
                    SetMetaData(hit.collider.gameObject);
                }
            }
        }
    }

    public void ToggleUI()
    {
        UIPanel.SetActive(!UIPanel.activeSelf);
    }

    void SetMetaData(GameObject go)
    {
        //Set the metadata panel with data of corresponding gameobject
        MetaDataPanel.GetComponentInChildren<TextMeshProUGUI>().text = go.GetComponent<ARObject>().GetMetaData();
        ChangeMaterials(go, SelectedMaterial);

       
    }

    public void ChangeColor(string GOName)
    {

        //if appcontroller private space then change material of private space or else change material of shared space go

        if(m_AppController.privateSpace)
        {
            Transform parentGO = GameObject.FindGameObjectWithTag("Private Space").transform;

            if (parentGO != null)
            {
                foreach (Transform child in parentGO)
                {

                    if (child.name == GOName)
                    {
                        ChangeMaterials(child.gameObject, ColorChangeMaterial);
                    }
                }
            }
            else
            {
                m_MessageBox.SetActive(true);
                m_MessageBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{parentGO.name} not found ";
            }
        }
        else
        {
            Transform parentGO = GameObject.FindGameObjectWithTag("Shared Space").transform;
            if (parentGO != null)
            {
                foreach (Transform child in parentGO)
                {

                    if (child.name == GOName +"(Clone)")
                    {
                        ChangeMaterials(child.gameObject, ColorChangeMaterial);


                    }
                    //send rpc to change color of the object on all clients
                }
            }
            else
            {
                m_MessageBox.SetActive(true);
                m_MessageBox.GetComponentInChildren<TextMeshProUGUI>().text = $"{parentGO.name} not found ";
            }
        }
        
    }

    private void ChangeMaterials(GameObject go, Material mat)
    {
        //add or remove selected material to selected gameobject mesh renderer
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        ARObject aRObject = go.GetComponent<ARObject>();

        if (meshRenderer.materials.Length > 1)
        {
            meshRenderer.materials = new Material[] { meshRenderer.materials[0] };
            aRObject.m_ObjectColor.Value = meshRenderer.materials[0].color;
        }
        else
        {
            meshRenderer.AddMaterial(mat);
            aRObject.m_ObjectColor.Value = mat.color;
        }
    }

}
