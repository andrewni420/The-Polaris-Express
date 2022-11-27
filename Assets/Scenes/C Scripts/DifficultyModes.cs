using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// tutorial used: https://www.youtube.com/watch?v=5onggHOiZaw

public class DifficultyModes : MonoBehaviour
{
	int val = 0; 

	public void modeVal(int index){
		val = index;
	}
	public void StartGame()
    {  
        Debug.Log(val);
        if (val ==0){
        	Debug.Log("Easy");
			SceneManager.LoadScene("Main");
		}
		//Medium Mode
		if (val ==1){
			Debug.Log("Medium");
			SceneManager.LoadScene("Main");
		}
		//Hard Mode
		if (val ==2){
			Debug.Log("Hard");
			SceneManager.LoadScene("Main");
		}
        
    }


}
