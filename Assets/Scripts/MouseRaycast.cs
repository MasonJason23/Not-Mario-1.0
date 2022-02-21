using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(mouseRay, out RaycastHit hit))
        {
            Debug.Log("Hit!");
        }
    }
}
