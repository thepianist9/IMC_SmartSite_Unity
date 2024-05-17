using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleEdit : MonoBehaviour
{
    private Image image;
    private Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        toggle = GetComponent<Toggle>();
    }

    public void ToggleEdits()
    {
        if (toggle.isOn)
        {
            image.color = new Color(0, 1, 0.9f);

        }
        else
        {
            image.color = new Color(0, 0.6f, 1);
        }
    }

   
}
