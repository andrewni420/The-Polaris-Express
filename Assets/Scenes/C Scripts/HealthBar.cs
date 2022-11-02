using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Tutorial: https://www.youtube.com/watch?v=BLfNP4Sc_iA
// Connects player health values to physical bar on screen

public class HealthBar : MonoBehaviour
{
    // Connects to  slider to fill 
    public Slider slider;

    public void SetHealth(int health){

    	slider.value = health;
    }

    public void SetMaxHealth(int health){

    	slider.maxValue = health;
    	slider.value = health;
    }
}
