using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRSpatiotemopralAuthoring
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private List<GameObject> CircularUIList3D;
        [SerializeField] private GameObject DescriptionPanel;
        private int CircularUIIndex3D = 0;
        private GameObject CircularUIElement;


        private static UIManager _Instance;
        [SerializeField] private TPPCameraSwitcher tppCameraSwitcher;

        public static UIManager Instance { get { return _Instance; } }



        void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
            }

           
        }

        private void Start()
        {
            CircularUIElement = CircularUIList3D[CircularUIIndex3D];
            CircularUIElement.SetActive(true);
        }



        // Update is called once per frame
        void Update()
        {
            if (PlatformManager.Instance.platform == PlatformManager.Platform.Desktop || PlatformManager.Instance.platform == PlatformManager.Platform.Editor)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    CircularUIActivation(false);
                }
                //swap UI
                if (Input.GetKeyDown(KeyCode.N))
                {
                    CircularUIActivation(true);
                }
            }
        }

        public void CircularUIActivation(bool switchNext)
        {
            if(DescriptionPanel.activeSelf)
            {
                DescriptionPanel.SetActive(false);
            }
            if (switchNext)
            {
                Debug.Log("Switching to next UI element");
                int totalElements = CircularUIList3D.Count;

                // Deactivate all elements in the list
                foreach (GameObject element in CircularUIList3D)
                {
                    element.SetActive(false);
                }

                // Activate the current element and update the index
                CircularUIElement = CircularUIList3D[CircularUIIndex3D];
                CircularUIElement.SetActive(true);

                CircularUIIndex3D = (CircularUIIndex3D + 1) % totalElements;
            }
            else
            {

                CircularUIElement.SetActive(!CircularUIElement.activeSelf);
            }

        }
        public void SwitchContextMenu()
        {
            tppCameraSwitcher.SetTPPView();
            CircularUIActivation(false);
        }
        public void SwitchDescriptionPanel()
        {
            SwitchContextMenu();
            int descriptiondex = CircularUIList3D.IndexOf(DescriptionPanel);


            CircularUIList3D[descriptiondex].SetActive(true);
            CircularUIIndex3D = CircularUIList3D.IndexOf(DescriptionPanel) - 1;

        }


    }
    }

