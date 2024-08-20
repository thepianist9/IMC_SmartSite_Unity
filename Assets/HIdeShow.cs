using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HIdeShow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject DrawerMenu;
    [SerializeField] private int animationTimeMultiplier = 1;


    [SerializeField] private Vector3 initialPosition;
    private Vector3 targetPosition;

    [SerializeField] private Vector3 moveOffset = new Vector3(-50,0,0);


    private void Start()
    {
        initialPosition = DrawerMenu.transform.position;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("PointerEnter");
/*        DrawerMenu.SetActive(true);*/
        StartCoroutine(FadeUI(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("PointerExit");
        /*DrawerMenu.SetActive(false);*/
        StartCoroutine(FadeUI(false));
        /*AnimationInOut(false);*/
    }



    IEnumerator FadeUI(bool moveOut)
    {
        
        targetPosition = moveOut ? initialPosition : initialPosition + moveOffset;

        Vector3 startPos = moveOut ? initialPosition : targetPosition;
        Vector3 endPos = moveOut ? targetPosition : initialPosition;

        float duration = 1.0f / animationTimeMultiplier;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            DrawerMenu.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }
}
