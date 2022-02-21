using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRaycast : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Primary Btn not clicked");
        }
        else
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(mouseRay, out RaycastHit hit);

            if (hit.collider != null)
            {
                // Debug.Log(hit);
                Destroy(hit.transform.gameObject);
            }
        }
    }
}
