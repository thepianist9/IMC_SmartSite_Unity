using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnOnSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NetworkObject>().Spawn();  
    }

}
