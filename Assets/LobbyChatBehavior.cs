using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyChatBehavior : NetworkBehaviour
{
    [SerializeField] private ChatMessage chatMessagePrefab;
    [SerializeField] private Transform messageParent;
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatPanel;

    private const int MaximumMessages = 10;
    private List<ChatMessage> messages;

    private const float MinIntervalBetweenChatMessages = 1f;
    private float _clientSendTimer;


    private void Start()
    {
        messages = new List<ChatMessage>();

    }

    public void ToggleChat(bool isConnected)
    {
        chat.SetActive(isConnected);
    }

    public void ToggleChatUI()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }


    private void Update()
    {
        _clientSendTimer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(chatInputField.text.Length > 0 && _clientSendTimer > MinIntervalBetweenChatMessages)
            {
                SendMessage();
                chatInputField.DeactivateInputField(clearSelection: true);
            }
            else
            {
                chatInputField.Select();
                chatInputField.ActivateInputField();
            }
        }
    }

    public void SendMessage()
    {
        string message = chatInputField.text;
        chatInputField.text = "";

        if (string.IsNullOrEmpty(message))
            return;

        _clientSendTimer = 0;
        SendMessageServerRPC(message, NetworkManager.Singleton.LocalClientId);
    }

    private void AddMessage(string message, ulong NetworkId)
    {
        var msg = Instantiate(chatMessagePrefab, messageParent);
        string username = "Client_" + NetworkId;
        msg.SetMessage(username, message);

        messages.Add(msg);

        if(messages.Count > MaximumMessages) {
            Destroy(messages[0]);
            messages.RemoveAt(0);
        }

        
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageServerRPC(string message, ulong localClientId)
    {
        ReceiveChatMessageClientRPC(message, localClientId);
    }
    [ClientRpc]
    private void ReceiveChatMessageClientRPC(string message, ulong localClientId)
    {
        AddMessage(message, localClientId);
    }
}
