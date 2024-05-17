using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject NavMenu;
    [SerializeField] private GameObject AuthoringPanel;
    [SerializeField] private GameObject DescriptionPanel;

    public static NetworkingUIManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ToggleAuthoringPanel()
    {
        AuthoringPanel.SetActive(!AuthoringPanel.activeSelf);
        NavMenu.SetActive(!NavMenu.activeSelf);
        DescriptionPanel.SetActive(!DescriptionPanel.activeSelf);
    }
}
