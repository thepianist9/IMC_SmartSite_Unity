
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Assets.David.UILayout
{
    public class LayoutUIManager : MonoBehaviour
    {
        [Header("UI Layout Mode")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool worldCanvasMode = false;
        [SerializeField] private bool fullScreenMode = false;
        [SerializeField] private bool modalMode = false;

        [Header("Layout Configuration")]
        [SerializeField] private bool removeHeader = false;
        [SerializeField] private bool removeFooter = false;
        [SerializeField] private bool removeLeftBar = false;
        [SerializeField] private bool removeRightBar = false;

        [Header("World Canvas Settings")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject UILayoutBG;
        [SerializeField] private Vector3 canvasPosition = Vector3.zero;
        [SerializeField] private Vector3 canvasScale = new Vector3(0.01f, 0.01f, 0.01f);

        [Header("Layout Objects")]

        [SerializeField] private GameObject Modal;
        [SerializeField] private GameObject header;
        [SerializeField] private GameObject footer;
        [SerializeField] private GameObject leftDrawer;
        [SerializeField] private GameObject rightDrawer;

        [Header("Debug Objects")]
        [SerializeField] private List<GameObject> layoutContainerObjects;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
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
            foreach (GameObject layoutContainerObject in layoutContainerObjects)
            {
                if (layoutContainerObject is not null)
                {

                    layoutContainerObject.GetComponent<Image>().enabled = mode;
                }
            }
        }

        public void SetModalMode(bool modalMode)
        {
       
            Debug.Log("Modal Mode: " + modalMode);
            
            Modal.SetActive(modalMode);
            UILayoutBG.SetActive(modalMode);


            
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

    }
}
