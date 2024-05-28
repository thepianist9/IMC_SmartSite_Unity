using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPPCameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject tppCamera;
    [SerializeField] private GameObject tppMenuCamera;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SetTPPView();
        }
        if(tppMenuCamera.activeSelf)
        {
            starterAssetsInputs.cursorInputForLook = false;
        }
        else
        {
            starterAssetsInputs.cursorInputForLook = true;
        }
    }

    public void SetTPPView()
    {
        tppCamera.SetActive(!tppCamera.activeSelf);
        tppMenuCamera.SetActive(!tppMenuCamera.activeSelf);
    }
}
