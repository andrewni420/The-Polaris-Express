using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public GameObject pausePanel;

    // Called when the button is clicked
    public void OnButtonClick()
    
    {
        // Close the pause panel and unpause the game
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }
}
