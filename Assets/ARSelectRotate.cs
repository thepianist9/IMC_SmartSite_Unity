using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSelectRotate : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up * Time.deltaTime * 5f, Space.Self);   
    }
}
