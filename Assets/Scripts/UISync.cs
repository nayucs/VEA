using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UISync : MonoBehaviour
{
    Camera HMDCamera = Camera.main;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = HMDCamera.transform.position;
        transform.rotation = HMDCamera.transform.rotation;
    }
}
