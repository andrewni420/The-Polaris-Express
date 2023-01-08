using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// tutorial used: https://www.youtube.com/watch?v=5onggHOiZaw

public class DifficultyModes : MonoBehaviour
{
	public static string diff;
	public difficulty gamval;

	string[] diffTypes = {"peaceful", "easy", "medium", "hard"};

	int val = 0; 

	public void modeVal(int index){
		val = index;
	}
	public void StartGame()
    {  
    	diff = diffTypes[val];
        
        PlayerPrefs.SetString("difficulty", diff);
		SceneManager.LoadScene("Main");

        
    }


}
