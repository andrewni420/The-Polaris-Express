using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Died : MonoBehaviour
{
    public void Restart()
    {
        Debug.Log("Player died, hit restart and sent to Main Scene");
        SceneManager.LoadScene("Main");
    }
}
