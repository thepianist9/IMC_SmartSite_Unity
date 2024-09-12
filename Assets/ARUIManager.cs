using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUIManager : MonoBehaviour
{
    [SerializeField] public GameObject onlinePanel;
    [SerializeField] public GameObject offlinePanel;

    public void ToggleGameObject(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void SwitchOnline(bool online)
    {
        onlinePanel.SetActive(online);
        offlinePanel.SetActive(!online);
    }
}
