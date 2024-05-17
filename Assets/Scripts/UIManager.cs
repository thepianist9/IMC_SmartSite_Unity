using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSpatiotemopralAuthoring
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Camera XRCamera;
        [SerializeField] private GameObject twoDUI;
        [SerializeField] private GameObject threeDUI;
        [SerializeField] private GameObject threeDGraphPanel;
        [SerializeField] private GameObject spatialThreeDPanel;
        [SerializeField] private GameObject timelinePanel;


        [SerializeField] private GameObject AR_CameraRig;
        [SerializeField] private GameObject VR_CameraRig;

        [SerializeField] private GameObject EnviromentObject;
        //[SerializeField] private GameObject fpvcontroller;

        private static UIManager _Instance;
        public static UIManager Instance { get { return _Instance; } }



        
        public enum Platform
        {
            Mobile,
            VR,
            AR,
            Desktop,
            Editor
        }
        [SerializeField] public Platform platform;

        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }
            //Mobile
            if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
            {
                platform = Platform.AR;
                SetARMode();

            }
            //Editor
            else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.LinuxEditor))
            {
                platform = Platform.VR;

                //Set2dMode();
            }
            //Standalone Desktop
            else if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.LinuxPlayer))
            {
                platform = Platform.Desktop;
                SetARMode();
            }
            Debug.Log(platform.ToString());
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
                    //toggle UI
                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        if (threeDGraphPanel.activeSelf == false && spatialThreeDPanel.activeSelf == false)
                            spatialThreeDPanel.SetActive(true);
                        threeDUI.SetActive(!threeDUI.activeSelf);
                    }
                    //swap UI
                    if (Input.GetKeyDown(KeyCode.N))
                    {
                        Debug.Log("N pressed");
                        threeDGraphPanel.SetActive(!threeDGraphPanel.activeSelf);
                        spatialThreeDPanel.SetActive(!spatialThreeDPanel.activeSelf);

                    }
                    break;
                case Platform.AR:
                    //AR UI
                    break;


                case (Platform.Editor):

                        //toggle UI
                        if (Input.GetKeyDown(KeyCode.M))
                        {
                            if (threeDGraphPanel.activeSelf == false && spatialThreeDPanel.activeSelf == false)
                                spatialThreeDPanel.SetActive(true);
                            threeDUI.SetActive(!threeDUI.activeSelf);
                        }
                        //swap UI
                        if (Input.GetKeyDown(KeyCode.U))
                        {
                        Debug.Log("N pressed");
                            threeDGraphPanel.SetActive(!threeDGraphPanel.activeSelf);
                            spatialThreeDPanel.SetActive(!spatialThreeDPanel.activeSelf);

                        }
                        //RaycastUI3D();

                   
                    break;

                case Platform.Desktop:
                    //Desktop UI
                    if (threeDUI != null)
                    {
                        if (Input.GetKeyDown(KeyCode.M))
                        {
                            threeDUI.SetActive(!threeDUI.activeSelf);
                        }
                        //RaycastUI3D();

                    }
                    break;
                default:
                    /*print("[UI Manager]: Unable to recognize Platform");*/
                    break;
            }
        }

        private void SetARMode()
        {
            //remove and 3d Objects
            twoDUI.SetActive(true);
            EnviromentObject.SetActive(false);
            //fpvcontroller.SetActive(false);
            if(AR_CameraRig.activeSelf == false)
            {
                AR_CameraRig.SetActive(true);
            }
            
            if(VR_CameraRig.activeSelf == true)
            {
                VR_CameraRig.SetActive(false);
            }
            

            //set camera for ar
            //spawn building
        } 

        private void Set2dMode()
        {
            twoDUI.SetActive(true);
        }
        private void SetVRMode()
        {
            //remove and 3d Objects
            threeDUI.SetActive(true);
            EnviromentObject.SetActive(true);
            //fpvcontroller.SetActive(true);
            if (AR_CameraRig.activeSelf == true)
            {
                AR_CameraRig.SetActive(false);
            }

            if (VR_CameraRig.activeSelf == false)
            {
                VR_CameraRig.SetActive(true);
            }
            //set camera for ar
            //spawn building
        }

        public void ToggleTimeline_3D()
        {
            timelinePanel.SetActive(!timelinePanel.activeSelf);
        }
        public void HandleDropdownBlocker()
        {
            GameObject blocker = GameObject.Find("Blocker");
            if(blocker != null)
            {
                blocker.SetActive(false);
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

