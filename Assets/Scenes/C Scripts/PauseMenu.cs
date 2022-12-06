using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//https://www.youtube.com/watch?v=JivuXdrIHK0

public class PauseMenu : MonoBehaviour
{


	public static bool GameIsPaused = false;

    public void MainMenu(){
    	SceneManager.LoadScene("Welcome");
    }

    public void EndGame(){
    	SceneManager.LoadScene("Lose Menu");
    }

    public void Instructions(){
    	//need to create instruction scene 
    	
    }

    void Update(){
    	if(Input.GetKeyDown(KeyCode.Period)){
    		if(GameIsPaused){
    
    			

    		}

    	}
    }

}
