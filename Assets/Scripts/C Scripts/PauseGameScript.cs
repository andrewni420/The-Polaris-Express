using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameScript : MonoBehaviour
{
    // Panel game object
    public GameObject pausePanel;

    // Canvas game object
    public GameObject playerCanvas;

    void Update()
    {
        // Check if the "KeyCode.Tab" key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Toggle the pause panel
            if (pausePanel.activeSelf)
            {
                // Unpause the game and hide the panel
                Time.timeScale = 1;
                pausePanel.SetActive(false);

                // Reactivate the player canvas
                playerCanvas.SetActive(true);
            }
            else
            {
                // Pause the game and show the panel
                Time.timeScale = 0;
                pausePanel.SetActive(true);

                // Deactivate the player canvas
                playerCanvas.SetActive(false);
            }
        }
    }
}
