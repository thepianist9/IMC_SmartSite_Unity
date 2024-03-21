using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XRSpatiotemopralAuthoring;


public class ARSpawnManager : MonoBehaviour
{
    [SerializeField] ARRaycastManager m_ARRaycastManager;
    [SerializeField] ARPlaneManager m_ARPlaneManager;

    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField] GameObject m_SpawnablePrefab;
    [SerializeField] Image m_feedbackOutline;


    /// <summary>
    /// Event invoked after an object is spawned.
    /// </summary>
    /// <seealso cref="TrySpawnObject"/>
    public event Action<GameObject> objectSpawned;

    private bool isARMode = false;

    GameObject spawnedObject;
    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
    }

    public void SetARMode(GameObject go)
    {
        isARMode = true;
        m_SpawnablePrefab = go;
    }

    // Update is called once per frame
    void Update()
    {
        if (isARMode)
        {
            if (Input.touchCount == 0)
            {
                return;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (m_ARRaycastManager.Raycast(Input.GetTouch(0).position, m_Hits, TrackableType.Planes))
                {
                   
                    if (TrySpawnObject(m_Hits[0].pose))
                    {
                        if (m_SpawnablePrefab.name == "Graph")
                        {
                            //get hit position to place graphs
                            GraphManager.Instance.m_GraphPose = m_Hits[0].pose;
                        }

                        isARMode = false;
                        //feedback to spawn
                        TransitionColor(Color.clear, Color.green, 0.5f);
                        TransitionColor(Color.green, Color.clear, 0.5f);

                    }
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

    }

    public bool TrySpawnObject(Pose pose)
    {
        if (m_SpawnablePrefab != null)
        {
            GameObject visualizationGO = Instantiate(m_SpawnablePrefab, pose.position, pose.rotation);
            visualizationGO.AddComponent<ARAnchor>();

            Debug.Log("Object spawned");
            return true;

        }

        else return false;
    }

    IEnumerator TransitionColor(Color fromColor, Color toColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            m_feedbackOutline.color = Color.Lerp(fromColor, toColor, elapsedTime / duration);
            yield return null;
        }


    }

    public void TogglePlaneDetection()
    {
        m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;

        string planeDetectionMessage = "";
        if (m_ARPlaneManager.enabled)
        {
            planeDetectionMessage = "Disable Plane Detection and Hide Existing";
            m_ARPlaneManager.detectionMode = PlaneDetectionMode.None;
            m_ARPlaneManager.SetTrackablesActive(false);
        }
        else
        {
            planeDetectionMessage = "Enable Plane Detection and Show Existing";
            m_ARPlaneManager.detectionMode = PlaneDetectionMode.Horizontal;
            m_ARPlaneManager.SetTrackablesActive(true);
        }

    }

}


