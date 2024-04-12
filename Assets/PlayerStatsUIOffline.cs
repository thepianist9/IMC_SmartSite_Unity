using MongoDB.Driver.Core.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStatsUIOffline : MonoBehaviour
{
    public static PlayerStatsUIOffline Singleton { get; private set; }
    [SerializeField] TMP_Text _PlayerPanel;

    void OnEnable()
    {
        if (Singleton != null)
        {
            // As long as you aren't creating multiple NetworkManager instances, throw an exception.
            // (***the current position of the callstack will stop here***)
            throw new Exception($"Detected more than one instance of {nameof(PlayerStatsUIOffline)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

        private void Start()
    {
        DisplayOfflinePanel();
    }



    private void DisplayOfflinePanel()
    {
        _PlayerPanel.text = $"UserName: {ServerCheck.Instance.LocalClient.userName}";
    }
    public void SetOnlineMode()
    {
        NetworkSystemControl.Singleton.IsOnline = true;
        ServerCheck.Instance.ReadServerDB();
        //SceneTransitionHandler.sceneTransitionHandler.ExitAndLoadStartMenu();
    }

}
