using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


namespace XRSpatiotemopralAuthoring
{
    public class PlatformManager : MonoBehaviour
    {

        [SerializeField] private GameObject Client;
        
        //UI
        [SerializeField] private GameObject twoDUI;

        //CameraRigs
        [SerializeField] private GameObject thirdPersonCamera;
        [SerializeField] private GameObject AR_CameraRig;
        [SerializeField] private GameObject VR_CameraRig;

        [SerializeField] private GameObject Environment;
        //[SerializeField] private GameObject fpvcontroller;

        private static PlatformManager _Instance;
        public static PlatformManager Instance { get { return _Instance; } }




        public enum Platform
        {
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
            // Platform: Headset VR Oculus / COMPLETELY VIRTUAL
            if (Application.platform == RuntimePlatform.Android && CheckOculusRuntime())
            {
                Debug.Log("Running on Oculus Runtime"); 
                platform = Platform.VR;
                SetVRMode();
            }
            // Platform: Mobile / AUGMENTED REALITY
            else if ((Application.platform == RuntimePlatform.Android) || (Application.platform == RuntimePlatform.IPhonePlayer))
            {   
                platform = Platform.AR;
                SetARMode();
            }
            // Platform: Editor COMPLETELY VIRTUAL
            else if ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.LinuxEditor))
            {
                platform = Platform.Editor;
                SetDesktopMode();

            }
            //  Platform: Standalone Desktop COMPLETELY VIRTUAL
            else if ((Application.platform == RuntimePlatform.WindowsPlayer) || (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.LinuxPlayer))
            {
                platform = Platform.Desktop;
                SetDesktopMode();
            }
            Debug.Log(platform.ToString());
        }



       
        private bool CheckOculusRuntime()
        {
            // Get the XR device currently in use
            var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(xrDisplaySubsystems);
            if (xrDisplaySubsystems != null && xrDisplaySubsystems.Count > 0)
            {

                foreach (var xrDisplaySubsystem in xrDisplaySubsystems)
                {
                    if (xrDisplaySubsystem.running)
                    {
                        string subsystemName = xrDisplaySubsystem.SubsystemDescriptor.id;
                        Debug.Log("XR Display Subsystem Name: " + subsystemName);
                        if (subsystemName.Contains("Oculus"))
                        {
                            Debug.Log("Running on Oculus Runtime");
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }

        }
        private void SetARMode()
        {
            //remove and 3d Objects
            Set2DUI();
            Environment.SetActive(false);
            //fpvcontroller.SetActive(false);
            if (AR_CameraRig.activeSelf == false)
            {
                AR_CameraRig.SetActive(true);
            }
            if (Client.activeSelf == true)
            {
                Client.SetActive(false);
            }
        }

        private void SetVRMode()
        {

            if (AR_CameraRig.activeSelf == true)
            {
                AR_CameraRig.SetActive(false);
            }
            if (thirdPersonCamera.activeSelf == true)
            {
                thirdPersonCamera.SetActive(false);
            }

            VR_CameraRig.SetActive(true);
            Client.SetActive(true);
            Environment.SetActive(true);
        }
        private void SetDesktopMode()
        {
            if (AR_CameraRig.activeSelf == true)
            {
                AR_CameraRig.SetActive(false);
            }
            if (VR_CameraRig.activeSelf == true)
            {
                VR_CameraRig.SetActive(false);
            }

            thirdPersonCamera.SetActive(true);
            Client.SetActive(true);
            Environment.SetActive(true);
        }

        private void Set2DUI()
        {
            twoDUI.SetActive(true);
        }
      

    }
}

