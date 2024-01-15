using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleRotator : MonoBehaviour
{
    Vector3 angle = new Vector3(0, 0.5f, 0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(angle);
    }
}
