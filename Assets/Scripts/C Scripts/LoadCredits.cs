using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadCredits : MonoBehaviour
{
    public void CreditScene(){
    	Time. timeScale = 1;
    	SceneManager.LoadScene("Credits");
    }
}
