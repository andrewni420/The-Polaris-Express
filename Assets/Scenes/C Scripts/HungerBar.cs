using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Slider slider;

    public void SetHunger(int hunger){

    	slider.value = hunger;
    }

    public void SetMaxHunger(int hunger){

    	slider.maxValue = hunger;
    	slider.value = hunger;
    }
}
