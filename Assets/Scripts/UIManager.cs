using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSpatiotemopralAuthoring
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject UIGameObject;
        [SerializeField] private Camera XRCamera;


        [SerializeField]
        private enum Platform
        {
            Mobile,
            VR,
            AR,
            Desktop,
            Editor
        }
        Platform platform;
        // Start is called before the first frame update
        void Start()
        {
            //Mobile
            if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
            {
                platform = Platform.Mobile;
            }
            //Editor
            else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.LinuxEditor))
            {
                platform = Platform.Editor;
            }
            //Standalone Desktop
            else if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.LinuxPlayer))
            {
                platform = Platform.Desktop;
            }


        }

        // Update is called once per frame
        void Update()
        {
            switch (platform)
            {
                case Platform.Mobile:
                    //Mobile UI
                    if (Screen.orientation == ScreenOrientation.LandscapeLeft)
                    {
                        XRCamera.fieldOfView = 50;
                    }
                    else if (Screen.orientation == ScreenOrientation.LandscapeLeft)
                    {
                        XRCamera.fieldOfView = 70;
                    }
                    break;


                case Platform.VR:
                    //VR UI
                    break;
                case Platform.AR:
                    //AR UI
                    break;


                case (Platform.Editor):
                    if (UIGameObject != null)
                    {
                        if (Input.GetKeyDown(KeyCode.M))
                        {
                            UIGameObject.SetActive(!UIGameObject.activeSelf);
                        }
                        //RaycastUI3D();

                    }
                    break;

                case Platform.Desktop:
                    //Desktop UI
                    if (UIGameObject != null)
                    {
                        if (Input.GetKeyDown(KeyCode.M))
                        {
                            UIGameObject.SetActive(!UIGameObject.activeSelf);
                        }
                        //RaycastUI3D();

                    }
                    break;
                default:
                    print("[UI Manager]: Unable to recognize Platform");
                    break;
            }
        }

        /*private void RaycastUI3D()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit: " + hit.collider.name);
                }
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the ray hits a 3D UI element
                    if (hit.collider.CompareTag("UIElement"))
                    {
                        // Handle interaction with the UI element
                        Debug.Log("Hit UI Element: " + hit.collider.gameObject.name);
                        // Example: Trigger UI interaction method
                        hit.collider.GetComponent<UIEleme>().Interact();
                    }
                }
        }*/

    }
    }

