using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AuthoringUIManager : NetworkBehaviour 
{

    [SerializeField] private GameObject _ButtonGroup;
    [SerializeField] private GameObject Model;
    [SerializeField] private Button _SpawnModelBtn;
    [SerializeField] private Button _SpawnCubeBtn;

    [SerializeField] private int spawnYOffset;
    [SerializeField] private int spawnZOffset;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }

  

    private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
    {
        if(scene.name == "OfflineSession")
        {
            _SpawnModelBtn.onClick.AddListener(SpawnModel);
            _SpawnCubeBtn.onClick.AddListener(SpawnCube);
        } if(scene.name == "NetworkedSession")
        {
            _SpawnModelBtn.onClick.AddListener(SpawnModelNetworked);
            _SpawnCubeBtn.onClick.AddListener(SpawnCubeNetworked);
        }

    }
  


    private Vector3 GetPlayerPosition()
    {
        return GameObject.Find("PlayerArmature_Offline").transform.position;
    }private quaternion GetPlayerRotation()
    {
        return GameObject.Find("PlayerArmature_Offline").transform.rotation;
    }private Vector3 GetPlayerPositionNetworked()
    {
        return GameObject.Find("Player_" + NetworkManager.Singleton.LocalClientId).transform.position;
    }private quaternion GetPlayerRotationNetworked()
    {
        return GameObject.Find("Player_" + NetworkManager.Singleton.LocalClientId).transform.rotation;
    }

    private void SpawnModel()
    {
        Vector3 position = GetPlayerPosition() + Vector3.forward * spawnZOffset + Vector3.up * spawnYOffset;
        Quaternion rotation = GetPlayerRotation();
        //spawn
       GameObject go = Instantiate(Model, position, Quaternion.identity);
        go.GetComponent<Rigidbody>().isKinematic = false;
        go.GetComponent<NetworkObject>().enabled = false;
        go.GetComponent<NetworkTransform>().enabled = false;
    }     
    
    private void SpawnModelNetworked()
    {
        Vector3 position = GetPlayerPositionNetworked() + Vector3.forward * spawnZOffset + Vector3.up * spawnYOffset;
        Quaternion rotation = GetPlayerRotationNetworked();

        //spawn
        if(NetworkManager.Singleton.IsServer)
        {
            GameObject go = Instantiate(Model, position, Quaternion.identity);
            go.GetComponent<NetworkObject>().enabled = true;
            go.GetComponent<NetworkTransform>().enabled = true;
            go.GetComponent<NetworkObject>().Spawn();
        }
        else
        {
            //Send RPC to Server to spawn
            Debug.Log("[RPC]: Sending request to server");
            InitPrefabServerRpc("", position, rotation, NetworkManager.Singleton.LocalClientId);
        }
        
    }    

    private void SpawnCube()
    {
        Vector3 position = GetPlayerPosition() + Vector3.forward * spawnZOffset ;
        //spawn
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
   
    } 

    private void SpawnCubeNetworked()
    {
        Vector3 position = GetPlayerPositionNetworked() + Vector3.forward * spawnZOffset + Vector3.up * spawnYOffset;
        //spawn
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = position;
        NetworkObject no = cylinder.AddComponent<NetworkObject>();
        NetworkTransform nt = cylinder.AddComponent<NetworkTransform>();
        no.Spawn();
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void InitPrefabServerRpc(string craftPrefab, Vector3 position, Quaternion rotation, ulong id)
    {
        Debug.Log($"[RPC]: Instantiating prefab requested from client {id}");
        GameObject go = Instantiate(Model, position, Quaternion.identity);
        go.GetComponent<NetworkObject>().enabled = true;
        go.GetComponent<NetworkTransform>().enabled = true;
        go.GetComponent<NetworkObject>().Spawn();
    }



    public void HIdeShowAuthoringButtons()
    {
        _ButtonGroup.SetActive(!_ButtonGroup.activeSelf);
    }



    private void OnSceneUnloaded(Scene scene)
    {
        _SpawnModelBtn.onClick.RemoveAllListeners();
        _SpawnCubeBtn.onClick.RemoveAllListeners();

    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

}
