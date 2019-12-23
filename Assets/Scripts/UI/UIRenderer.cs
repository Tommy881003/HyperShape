using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRenderer : MonoBehaviour
{
    private Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = CameraFollower.instance.cam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
