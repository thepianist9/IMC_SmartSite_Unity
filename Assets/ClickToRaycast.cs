using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ClickToRaycast : MonoBehaviour
{
    [SerializeField] private GameObject MetaDataPanel;
    [SerializeField] private GameObject UIPanel;

    [SerializeField] private Material SelectedMaterial;
    [SerializeField] private Material ColorChangeMaterial;
     
    

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
        GameObject go = GameObject.Find(GOName);
        if(go != null)
        {
            ChangeMaterials(go, ColorChangeMaterial);
        }
        else
        {
            Debug.Log("GameObject not found");
        }
    }

    private void ChangeMaterials(GameObject go, Material mat)
    {
        //add or remove selected material to selected gameobject mesh renderer
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

        if (meshRenderer.materials.Length > 1)
        {
            meshRenderer.materials = new Material[] { meshRenderer.materials[0] };
        }
        else
        {
            meshRenderer.AddMaterial(mat);
        }
    }

}
