using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarAnimationOne : MonoBehaviour
{

    public PlayerMovement player1;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
        {
            Debug.Log("Player ran into a star 0");
            other.GetComponent<Animator>().SetTrigger("Sky");
        }
    }
}
