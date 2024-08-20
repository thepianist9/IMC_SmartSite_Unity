using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionManager : MonoBehaviour
{
    [SerializeField] TMP_Text descriptionText;
    // Start is called before the first frame update
    void Start()
    {
        if (descriptionText.text.Equals(""))
        {
            
            descriptionText.text = "No Object Selected";
        }
    }

}
