using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public Button exitButton; 
    public Button homeScreenButton;


    void Start()
    {

        exitButton.onClick.AddListener(Exit);
        homeScreenButton.onClick.AddListener(LoadHomeScreen);
    }

    void Exit()
    {
        Application.Quit();
    }

    void LoadHomeScreen()
    {
        SceneManager.LoadScene("Welcome");
    }
}

