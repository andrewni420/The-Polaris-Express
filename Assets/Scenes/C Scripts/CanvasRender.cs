using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRender : MonoBehaviour
{
    // The canvas to enable or disable rendering for
    public Canvas canvas;

    // A boolean value indicating whether the canvas should be rendered or not
    public bool shouldRender;

    void Update()
    {
        // Enable or disable rendering for the canvas based on the value of shouldRender
        canvas.enabled = false;
    }
}