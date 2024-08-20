using Unity.Netcode.Components;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

namespace XRSpatiotemopralAuthoring
{
    public class AuthoringManager : NetworkBehaviour
    {
        [SerializeField] private GameObject AuthoringPanel;
        private GameObject AuthoringGO;
        private SpatioControlManager spatioControlManager;
        private bool isAuthoringOn = false;
        private Ray ray;
        private RaycastHit hit;
        [SerializeField] private TMP_Text debugText;
        [SerializeField] private List<GameObject> gameObjects;
        [SerializeField] private int selectedGameObjectIndex;
        [SerializeField] private TMP_Text _DescriptionPanelText;
        [SerializeField] private TPPCameraSwitcher tPPCameraSwitcher;


        private void Start()
        {
            spatioControlManager = SpatioControlManager.Instance;
        }

        //method to set authoring content

        public void SetAuthoringContent(GameObject go)
        {
            //set authoring content and 
            if (go != null)
                AuthoringGO = go;
            //toggleUI
            ToggleAuthoringUI();
            selectedGameObjectIndex = gameObjects.IndexOf(go);
            Debug.Log(selectedGameObjectIndex);
            //SetAuthoring to active
            Author();
            if (spatioControlManager.GetDataFromID(go.name))
            {
                UIManager.Instance.SwitchDescriptionPanel();
            }
        }

        private void ToggleAuthoringUI()
        {
            if (AuthoringPanel != null && !SceneManager.GetActiveScene().name.Equals("Project Scene"))
            {
                UIManager.Instance.CircularUIActivation(false);
                tPPCameraSwitcher.SetTPPView();
            }
        }

        //method to author content

        private void Author()

        {
            if (AuthoringGO != null)
            {
                isAuthoringOn = true;
            }
        }

        //method to manage authoring functionality

        // Update is called once per frame
        void Update()
        {
            if (isAuthoringOn)
            {
                Mouse mouse = Mouse.current;
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    Vector3 mousePosition = mouse.position.ReadValue();
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        // Use the hit variable to determine what was clicked on.
                        Debug.Log($"hit object at {hit.transform.position}");
                        Debug.Log(SceneManager.loadedSceneCount);
                        //spawn object and revert authoring mode
                        if (SceneManager.loadedSceneCount > 1)
                        {
                            SpawnOnline(hit.transform.position);
                        }
                        else
                        {
                            SpawnOffline(hit.transform.position);
                        }
                    }

                }

            }
        }
        void SpawnOffline(Vector3 position)
        {

            //spawn object and revert authoring mode
            if (Instantiate(AuthoringGO, position, Quaternion.identity) != null)
            {
                isAuthoringOn = false;
                ToggleAuthoringUI();
            }


        }

        public void GetDataFromID(string name)
        {
            //we get the filtered attribute
            //we get the filter min max values_
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Debug.Log(DataManager.Instance._constructionBuildingComponents);

            foreach (var constructionObject in DataManager.Instance._constructionBuildingComponents)
            {
                if (constructionObject.name == name)
                {
                    dict.Add("Name", constructionObject.name);
                    dict.Add("Milestone", constructionObject.milestone);
                    dict.Add("Size", constructionObject.size.ToString());
                    dict.Add("Type", constructionObject.type);
                    dict.Add("Material", constructionObject.material);
                    dict.Add("Location", constructionObject.location);
                    dict.Add("Height", constructionObject.height.ToString());
                }
            }
            _DescriptionPanelText.text = "";
            foreach (var item in dict)
            {
                _DescriptionPanelText.text += item.Key + " : " + item.Value + "\n";
            }
        }
        void SpawnOnline(Vector3 position)
        {

            InitPrefabServerRpc(selectedGameObjectIndex, position, Quaternion.identity, NetworkManager.Singleton.LocalClientId);


        }

        [ServerRpc(RequireOwnership = false)]
        private void InitPrefabServerRpc(int GOIndex, Vector3 position, Quaternion rotation, ulong id)
        {

            ReceivePrefabMessageClientRPC(GOIndex, position, rotation, id);
            isAuthoringOn = false;
            ToggleAuthoringUI();
            debugText.text = $"Server spawning prefab with index {selectedGameObjectIndex}";

        }

        [ClientRpc]
        private void ReceivePrefabMessageClientRPC(int GOIndex, Vector3 position, Quaternion rotation, ulong id)
        {
            Debug.Log($"[RPC]: Instantiating prefab: {GOIndex} requested from client {id}");
            GameObject go = Instantiate(gameObjects[selectedGameObjectIndex], position, rotation);
            NetworkManager.Singleton.AddNetworkPrefab(go);
            go.GetComponent<NetworkObject>().enabled = true;
            go.GetComponent<NetworkTransform>().enabled = true;
            go.GetComponent<NetworkObject>().Spawn();
            //debugMessage from server to client
            debugText.text = $"Client with index {id} spawning prefab with index {selectedGameObjectIndex}";



        }

    }
}