using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// A script to set the color of each player based on OwnerClientId
/// </summary>

public class GameClient : NetworkBehaviour
{
    [SerializeField] public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] public NetworkVariable<Color> clientColor = new NetworkVariable<Color>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private string clientName;
    [SerializeField] private string m_clientColor;

    [SerializeField] public NetworkVariable<FixedString32Bytes> userName = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private void Awake()
    {

        Debug.Log("GAMECLIENT: Awake");
        Debug.Log($"NetworkVariable is {userName.Value} when spawned.");

        userName.OnValueChanged += OnSomeValueChanged;
        clientColor.OnValueChanged += OnColorValueChanged;
    }

    private void OnColorValueChanged(Color previousValue, Color newValue)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previousValue} | Current: {newValue}");
        m_clientColor = newValue.ToString();
        PlayerStatsUI.Singleton.UpdateOnlinePanel();
    }

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("GAMECLIENT: Start");
            clientId.Value = NetworkManager.Singleton.LocalClientId;
            userName.Value = ServerCheck.Instance.LocalClient.userName;
            clientName = userName.Value.ToString();
        }

        Debug.Log("GAMECLIENT: On NetworkSpawn");

    }

    private void OnSomeValueChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        Debug.Log($"Detected NetworkVariable Change: Previous: {previousValue} | Current: {newValue}");
        clientName = newValue.ToString();
        PlayerStatsUI.Singleton.UpdateOnlinePanel();
    }
}


