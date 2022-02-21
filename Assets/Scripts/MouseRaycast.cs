using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseRaycast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coins;
    private int coinTotal = 0;
    
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

            if (hit.collider == null)
            {
                
            }
            else if (hit.collider.tag.Equals("Brick"))
            {
                Debug.Log("Brick Destroyed");
                Destroy(hit.transform.gameObject);
            }
            else if (hit.collider.tag.Equals("?"))
            {
                Debug.Log("\'?\' Block Hit");
                coinTotal++;
                if (coinTotal.ToString().Length < 2)
                {
                    coins.text = "0";
                    coins.text += coinTotal.ToString();
                }
                else
                {
                    coins.text = coinTotal.ToString();
                }
            }
        }
    }
}
