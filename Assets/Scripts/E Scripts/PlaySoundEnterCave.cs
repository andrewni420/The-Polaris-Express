using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEnterCave : MonoBehaviour
{
    ////Used https://www.youtube.com/watch?v=yE0JdtVTnVk as a tutorial for this code

    public AudioSource echo;

    void Start()
    {
        echo = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cave")
        {
            echo.Play();
            Debug.Log("In Cave");
        }
        else
        {
            echo.Stop();
            Debug.Log("Not Cave");
        }
    }
}
