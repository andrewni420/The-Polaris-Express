using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarFillAmount : MonoBehaviour
{
    // The Image component to set the fill amount of
    public Image fillImage;

    // The current number of stars
    public int starCount;

    // The maximum number of stars
    public int maxStars;

    void Update()
    {
        // Calculate the fill amount as a value between 0 and 1
        float fillAmount = (float)starCount / (float)maxStars;

        // Set the fill amount of the Image component
        fillImage.fillAmount = fillAmount;
    }
}
