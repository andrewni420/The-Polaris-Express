using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WelcomeMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Button Clicked");
        SceneManager.LoadScene("Main");
    }
}
