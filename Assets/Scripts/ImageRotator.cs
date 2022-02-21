using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRotator : MonoBehaviour
{
    public float speed = 2f;
    public float degrees = 90f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * degrees * speed);
    }
}
