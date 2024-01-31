using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


public class ARSpawnManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager m_ARRaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField] GameObject m_SpawnablePrefab;

    GameObject spawnedObject;
    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }
        if (m_ARRaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                SpawnPrefab(Input.GetTouch(0).position);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = m_Hits[0].pose.position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                spawnedObject = null;
            }
        }
    } 

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        spawnedObject = Instantiate(m_SpawnablePrefab, spawnPosition, Quaternion.identity);
    }
}


