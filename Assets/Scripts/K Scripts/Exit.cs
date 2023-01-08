using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void Restart()
    {
        Debug.Log("Player won, hit restart and sent to Main Scene");
        SceneManager.LoadScene("Main");
    }
}
