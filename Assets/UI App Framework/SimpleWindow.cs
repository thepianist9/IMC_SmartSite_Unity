using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWindow : MonoBehaviour
{
    [SerializeField] private RectTransform rtTarget;
    [SerializeField] private RectTransform rtSelf;

    private bool maximize = true;
    // Start is called before the first frame update

    private void Start()
    {
        rtSelf = GetComponent<RectTransform>();
    }




    //Button when clicked calls function gets size and then adjusts the size based on the height and width of the rect transform with 
    //width = 60%
    //height = 40%

    public void SetSize()
    {
        SetCenter();
        if (!maximize)
        {
            rtTarget = GameObject.FindGameObjectWithTag("MainArea").GetComponent<RectTransform>();
            if (rtTarget.rect.size[0] != 0 && (rtTarget.rect.size[1] != 0))
            {
                rtSelf.sizeDelta = new Vector2(rtTarget.sizeDelta.x, rtTarget.sizeDelta.y);
            }
            maximize = !maximize;
        }
        else
        {
            rtTarget = GameObject.FindGameObjectWithTag("MainArea").GetComponent<RectTransform>();
            if (rtTarget.rect.size[0] != 0 && (rtTarget.rect.size[1] != 0))
            {
                rtSelf.sizeDelta = new Vector2(rtTarget.sizeDelta.x * 0.6f, rtTarget.sizeDelta.y * 0.6f);
            }
            maximize = !maximize;
        }
        

    }

    public void SetStretch()
    {
        if(rtSelf is not null)
        {
            rtSelf.anchorMin = new Vector2(0f, 0f);
            rtSelf.anchorMax = new Vector2(1f, 1f);
        }
    }
    public void SetCenter()
    {
        if(rtSelf != null)
        {
            rtSelf.anchorMin = new Vector2(0.5f, 0.5f);
            rtSelf.anchorMax = new Vector2(0.5f, 0.5f);

        }
    }

}
