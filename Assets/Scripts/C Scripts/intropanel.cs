using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class intropanel : MonoBehaviour
{
  // Assign the panel in the Inspector
    public GameObject panel;

    void Update()
    {
        // Check if the Enter key was pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Set the panel's active state to false
            panel.SetActive(false);
        }
    }
}
