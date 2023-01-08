using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewBehavior : MonoBehaviour
{
	// The scrollview gameobject
	public GameObject scrollView;
	// The exit button gameobject
	public GameObject exitButton;

	public void OpenPanel()
	{
    // Enable the scrollview and exit button gameobjects
    scrollView.SetActive(true);
    exitButton.SetActive(true);
	}

	public void ClosePanel()
	{
	    // Disable the scrollview and exit button gameobjects
	    scrollView.SetActive(false);
	    exitButton.SetActive(false);
	}
}