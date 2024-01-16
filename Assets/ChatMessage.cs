using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    public void SetMessage(string playerName, string message)
    {
        GetComponent<TMP_Text>().text = $"<color=grey>{playerName}</color>: {message}";
    }
}
