using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Tutorial: https://www.youtube.com/watch?v=BLfNP4Sc_iA
// Connects player hunger values to physical bar on screen

public class HungerBar : MonoBehaviour
{
	// Connects to  slider to fill 
    public Slider slider;

    public void SetHunger(int hunger){

    	slider.value = hunger;
    }

    public void SetMaxHunger(int hunger){

    	slider.maxValue = hunger;
    	slider.value = hunger;
    }
}
