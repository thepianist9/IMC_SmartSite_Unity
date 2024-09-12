//MIT License
//Copyright (c) 2023 DA LAB (https://www.youtube.com/@DA-LAB)
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RuntimeHandle;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

public class SelectTransformGizmo : MonoBehaviour
{
    [Header("Material for edit")]
    public Material highlightMaterial;
    public Material selectionMaterial;
    [SerializeField] private Image editSpaceImage;

    [Header("UI Colors")]
    [SerializeField] private Color NonEditColor;
    [SerializeField] private Color EditColor;


    private Material originalMaterialHighlight;
    private Material originalMaterialSelection;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    private RaycastHit raycastHitHandle;
    private GameObject runtimeTransformGameObj;
    private RuntimeTransformHandle runtimeTransformHandle;
    private int runtimeTransformLayer = 6;
    private int runtimeTransformLayerMask;
    private bool select = false;

    private void Start()
    {
        Reset();
    }
    private void Reset()
    {
        runtimeTransformGameObj = new GameObject();
        runtimeTransformHandle = runtimeTransformGameObj.AddComponent<RuntimeTransformHandle>();
        runtimeTransformGameObj.layer = runtimeTransformLayer;
        runtimeTransformLayerMask = 1 << runtimeTransformLayer; //Layer number represented by a single bit in the 32-bit integer using bit shift
        runtimeTransformHandle.type = HandleType.POSITION;
        runtimeTransformHandle.autoScale = true;
        runtimeTransformHandle.autoScaleFactor = 1.0f;
        runtimeTransformGameObj.SetActive(false);
    }

    void Update()
    {

        if (select)
        {
            Debug.Log("Entering Update loop");
            // Highlight
            if (highlight != null)
            {
                var renderer = highlight.GetComponent<MeshRenderer>();
                renderer.materials = new Material[] { (renderer.materials[0]) };
                highlight = null;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
            {
                highlight = raycastHit.transform;
                var renderer = highlight.GetComponent<MeshRenderer>();
                Debug.Log(highlight.gameObject.name);
                if (highlight.CompareTag("ARSpawnable") && highlight != selection)
                {
                    if (renderer.materials.Length ==  1)
                    {
                        originalMaterialHighlight = highlight.GetComponent<MeshRenderer>().materials[0];
                        if (renderer != null)
                            highlight.GetComponent<MeshRenderer>().AddMaterial(highlightMaterial);
                        else
                        {
                            MeshRenderer[] renderers = highlight.GetComponentsInChildren<MeshRenderer>();
                            foreach (MeshRenderer rendererc in renderers)
                            {
                                rendererc.AddMaterial(highlightMaterial);
                            }
                        }
                        }
                        
                    else if (renderer.materials.Length > 1) //if it has more than one material maybe selected before
                    {
                        if (renderer.materials[1] != highlightMaterial)
                        {
                            renderer.materials[1]  = highlightMaterial;
                        }
                    }
                }

                else
                {
                    highlight = null;
                }
            }

            // Selection
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log(selection);

                ApplyLayerToChildren(runtimeTransformGameObj);
                if (Physics.Raycast(ray, out raycastHit))
                {
                    if (Physics.Raycast(ray, out raycastHitHandle, Mathf.Infinity, runtimeTransformLayerMask)) //Raycast towards runtime transform handle only
                    {
                    }
                    else if (highlight)
                    {
                        if (selection != null)
                        {
                            selection.GetComponent<MeshRenderer>().AddMaterial(originalMaterialSelection);
                        }
                        selection = raycastHit.transform;
                        var meshRenderer = selection.GetComponent<MeshRenderer>();
                        if (selection.GetComponent<MeshRenderer>().materials[1] != selectionMaterial)
                        {
                            originalMaterialSelection = originalMaterialHighlight;
                            meshRenderer.AddMaterial(selectionMaterial);
                            runtimeTransformHandle.target = selection;
                            runtimeTransformGameObj.SetActive(true);
                        }
                        highlight = null;
                    }
                    else
                    {
                        if (selection)
                        {
                            var meshRenderer = selection.GetComponent<MeshRenderer>();
                            if (meshRenderer.materials.Length > 1)
                            {
                                meshRenderer.materials = new Material[] { meshRenderer.materials[0] };
                            }
                            selection = null;

                            runtimeTransformGameObj.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (selection)
                    {
                        var meshRenderer = selection.GetComponent<MeshRenderer>();
                        if (meshRenderer.materials.Length > 1)
                        {
                            meshRenderer.materials = new Material[] { meshRenderer.materials[0] };
                        }
                        selection = null;

                        runtimeTransformGameObj.SetActive(false);
                    }
                }
            }

            //Hot Keys for move, rotate, scale, local and Global/World transform
            if (runtimeTransformGameObj.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    runtimeTransformHandle.type = HandleType.POSITION;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    runtimeTransformHandle.type = HandleType.ROTATION;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    runtimeTransformHandle.type = HandleType.SCALE;
                }
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.G))
                    {
                        runtimeTransformHandle.space = HandleSpace.WORLD;
                    }
                    if (Input.GetKeyDown(KeyCode.L))
                    {
                        runtimeTransformHandle.space = HandleSpace.LOCAL;
                    }
                }
            }
        }

    }

    public void ToggleEdit()
    {
        select = !select;
        if(select)
        {
            editSpaceImage.color = EditColor;
            
        }
        else
        {
            editSpaceImage.color = NonEditColor;
            runtimeTransformHandle.type = HandleType.POSITION;
            runtimeTransformGameObj.SetActive(false);
        }

    }
    public void ChangeHandleType(string handleType)
    {
        switch (handleType)
        {
            case "Position":
                runtimeTransformHandle.type = HandleType.POSITION;
                break;
            case "Rotation":
                runtimeTransformHandle.type = HandleType.ROTATION;
                break;
            case "Scale":
                runtimeTransformHandle.type = HandleType.SCALE;
                break;
        }
    }


    private void ApplyLayerToChildren(GameObject parentGameObj)
    {
        foreach (Transform transform1 in parentGameObj.transform)
        {
            int layer = parentGameObj.layer;
            transform1.gameObject.layer = layer;
            foreach (Transform transform2 in transform1)
            {
                transform2.gameObject.layer = layer;
                foreach (Transform transform3 in transform2)
                {
                    transform3.gameObject.layer = layer;
                    foreach (Transform transform4 in transform3)
                    {
                        transform4.gameObject.layer = layer;
                        foreach (Transform transform5 in transform4)
                        {
                            transform5.gameObject.layer = layer;
                        }
                    }
                }
            }
        }
    }

}