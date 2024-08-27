using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XRSpatiotemopralAuthoring;

public class CharacterLookSwitch : MonoBehaviour
{
    private StarterAssetsInputs starterAssetsInputs;
    // Start is called before the first frame update
    void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            starterAssetsInputs.cursorInputForLook = !starterAssetsInputs.cursorInputForLook;
            Debug.Log("ping");
        }

    }

    void OnTriggerEnter(UnityEngine.Collider other)
    {

        if(SceneManager.loadedSceneCount > 1 )
        {
            if (other.gameObject.tag == "Shared Space")
            {
                AuthoringManager.Instance.sharedSpawn = true;
                Debug.Log("Setting shared spawn to true");
            }
            else if (other.gameObject.tag == "Private Space")
            {
                AuthoringManager.Instance.sharedSpawn = false;
                Debug.Log("Setting shared spawn to false");
            }
        }      
    }
    
    void OnTriggerExit(UnityEngine.Collider other)
    {
        if (SceneManager.loadedSceneCount > 1)
        {
            if (other.gameObject.tag == "Shared Space")
            {
                AuthoringManager.Instance.sharedSpawn = false;
                Debug.Log("Setting shared spawn to false");
            }
        }
        
    }
}
