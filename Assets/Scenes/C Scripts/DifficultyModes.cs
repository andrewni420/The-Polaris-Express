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

	string[] diffTypes = {"peaceful", "easy", "hard"};

	int val = 0; 

	public void modeVal(int index){
		val = index;
	}
	public void StartGame()
    {  
    	diff = diffTypes[val];
        Debug.Log(val);
       
		SceneManager.LoadScene("Main");
		
        
    }


}
