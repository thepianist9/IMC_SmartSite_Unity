using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
 
public class AuthoringUIPanel : MonoBehaviour
{
    [SerializeField] private ToggleGroup toggleGroup;
    public List<Toggle> toggles = new List<Toggle>();
    public List<GameObject> correspondingGameObjects = new List<GameObject>();
    [SerializeField] private TMP_Text header;
    // Start is called before the first frame update
    void Start()
    {

        foreach (Toggle toggle in toggles)
        {
            // Add a listener to the toggle's onValueChanged event
            toggle.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle); });
        }
    }


    private void ToggleValueChanged(Toggle toggle)
    {
        // Get the index of the toggled toggle
        int index = toggles.IndexOf(toggle);

        // Check if the index is valid and the corresponding GameObject exists
        if (index >= 0 && index < correspondingGameObjects.Count && correspondingGameObjects[index] != null)
        {
            if (correspondingGameObjects[index].name == "NetworkingToggle_Object")
            {
/*                PlayerStatsUIOffline.Singleton.SetOnlineMode();*/
/*                UIManager.Instance.SwitchUI();*/
            }
            // Set the corresponding GameObject active or inactive based on the toggle state
            correspondingGameObjects[index].SetActive(toggle.isOn);
           
        }
        if(toggle.isOn)
        {
            header.text = toggle.GetComponentInChildren<TMP_Text>().text;
        }
    }
}
