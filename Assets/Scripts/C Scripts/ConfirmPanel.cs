using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmPanel : MonoBehaviour
{	public void Start(){

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	public GameObject panel;
    public void OpenPanel(){
    	if(panel != null){
    		panel.SetActive(true);
    	}
    }
    public void ClosePanel(){
    	panel.SetActive(false);

    }
}
