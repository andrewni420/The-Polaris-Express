using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Died : MonoBehaviour
{
	public void Start(){

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
    public void Restart()
    {
        Debug.Log("Player died, hit restart and sent to Main Scene");
        SceneManager.LoadScene("Main");
    }
}
