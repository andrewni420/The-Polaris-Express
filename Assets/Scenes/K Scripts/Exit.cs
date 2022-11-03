using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class Exit : MonoBehaviour
{
	public void Start(){

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
    public void Quit()
    {
        Application.Quit();
    }
}
