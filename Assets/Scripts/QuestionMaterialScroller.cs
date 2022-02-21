using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMaterialScroller : MonoBehaviour
{
    private float accumulatedTime = 0f;
    // Update is called once per frame
    void Update()
    {
        accumulatedTime += Time.deltaTime;
        
        if (accumulatedTime >= 0.2f)
        {
            accumulatedTime = 0f;
            Material material = GetComponent<MeshRenderer>().material;
            material.mainTextureOffset += new Vector2(0f, 1 / 5f);
        }
    }
}
