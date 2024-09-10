/*
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.David.UILayout
{
    public class LayoutUIManager : MonoBehaviour
    {
        [Header("UI Layout Mode")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool worldCanvasMode = false;
        [SerializeField] private bool fullScreenMode = false;
        [SerializeField] private bool modalMode = false;

        [Header("Dark Mode")]
        [SerializeField] private bool darkMode = true;
        [SerializeField] private Color lightColor;
        [SerializeField] private Color darkColor;
        [SerializeField] private Color darkTextColor;
        [SerializeField] private Color lightTextColor;



        [Header("Debug Only")]
        [SerializeField] private GameObject debugWindow;
        [SerializeField] private Button debugButton;

        [Header("Layout Configuration")]
        [SerializeField] private bool removeHeader = false;
        [SerializeField] private bool removeFooter = false;
        [SerializeField] private bool removeLeftBar = false;
        [SerializeField] private bool removeRightBar = false;

        [Header("World Canvas Settings")]
        [SerializeField] private Canvas canvas;
        // [SerializeField] private GameObject UILayoutBG;
        [SerializeField] private Vector3 canvasPosition = Vector3.zero;
        [SerializeField] private Vector3 canvasScale = new Vector3(0.01f, 0.01f, 0.01f);

        [Header("Layout Objects")] 
        [SerializeField] private GameObject mainContent;
        [SerializeField] private GameObject modalPrefab;
        [SerializeField] private GameObject header;
        [SerializeField] private GameObject footer;
        [SerializeField] private GameObject leftDrawer;
        [SerializeField] private GameObject rightDrawer;

        [Header("Debug Objects")]
        [SerializeField] private List<GameObject> layoutContainerObjects = new List<GameObject>();
        [SerializeField] private List<GameObject> layoutContentObjects = new List<GameObject>();

        private GameObject modal;

        #region Singleton

        public static LayoutUIManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if(Instance != this) Destroy(this);
            DontDestroyOnLoad(this);
        }

        #endregion
        private void OnEnable()
        {
            if (Debug.isDebugBuild || debugMode == true)
            {
                Debug.Log("Debug Mode");
                debugButton.gameObject.SetActive(true);
                debugButton.onClick.AddListener(ToggleDebugWindow);
            }
        }

        private void OnValidate()
        {
            //Set Dark mode
            SetColorMode();

            SetModalMode(modalMode);

            //Debug Mode
            SetDebugMode(debugMode);
    
     
            //World Canvas Mode
            SetWorldCanvas(worldCanvasMode);


            //Layout Configuration editing
            EditLayoutConfig();

        }

        private void SetDebugMode(bool mode)
        {
            if (layoutContainerObjects is null || layoutContainerObjects.Count == 0) return;

            foreach (GameObject layoutContainerObject in layoutContainerObjects)
            {
                if (layoutContainerObject is not null)
                {
                    if (layoutContainerObject.TryGetComponent<Image>(out Image img))
                    {
                        img.enabled = mode;
                    }
                }
            }
        }

        public void SetModalMode(bool modalMode)
        {
            Debug.Log("Modal Mode: " + modalMode);
            if (modalMode)
            {
                modal = Instantiate(modalPrefab, mainContent.transform);
            }
            else if (modal != null)
            {
                Destroy(modal);
            }

            // UILayoutBG.GetComponent<Image>().enabled = modalMode;
        }

        private void SetWorldCanvas(bool mode)
        {
            if (canvas is not null)
            {
                if (worldCanvasMode)
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.position = canvasPosition;
                    canvas.transform.localScale = canvasScale;
                    canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
                }
                else
                {
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }
            else
            {
                Debug.Log("Canvas is not set");
            }

        }

        private void SetColorMode()
        {
            Color bgColor;
            Color imgColor;
            Color textColor;
            TMP_Text text = null;
            Image img;
 
            if(darkMode)
            {
                bgColor = darkColor;
                imgColor = darkColor;
                textColor = lightTextColor;
            }
            else
            {
                bgColor = lightColor;
                imgColor = lightColor;
                textColor = darkTextColor;
            }
            
            foreach (GameObject layoutContentObject in layoutContentObjects)
            {
                layoutContentObject.GetComponent<Image>().color = bgColor;
                foreach(Transform childTransform in  layoutContentObject.transform)
                {
                    text = childTransform.gameObject.GetComponent<TMP_Text>();
                    img = childTransform.GetComponent<Image>();
                    if(text != null)
                    {
                        text.color = textColor;
                    }
                    else if(img != null)
                    {
                        img.color = imgColor;
                    }

                }
            }
        }

        private void EditLayoutConfig()
        {
            if (header is null || footer is null || leftDrawer is null || rightDrawer is null)
            {
                Debug.Log("Layout Objects are not set");
                return;
            }
            else
            {
                if (fullScreenMode)
                {
                    header.SetActive(false);
                    footer.SetActive(false);
                    leftDrawer.SetActive(false);
                    rightDrawer.SetActive(false);

                }
                else
                {
                    header.SetActive(!removeHeader);
                    footer.SetActive(!removeFooter);
                    leftDrawer.SetActive(!removeLeftBar);
                    rightDrawer.SetActive(!removeRightBar);
                }

            }
        }

        public void SetUILayout(bool removeHeader, bool removeFooter, bool removeLeftBar, bool removeRightBar)
        {
            this.removeHeader = removeHeader;
            this.removeFooter = removeFooter;
            this.removeLeftBar = removeLeftBar;
            this.removeRightBar = removeRightBar;

            EditLayoutConfig();
        }

        public void SetFullScreen()
        {
            fullScreenMode = !fullScreenMode;
            EditLayoutConfig();
        }

        private void ToggleDebugWindow()
        {
            debugWindow.SetActive(!debugWindow.activeSelf);
        }

    }
}
*/