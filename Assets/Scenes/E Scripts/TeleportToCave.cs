using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToCave : MonoBehaviour
{
    public Transform teleportTarget;
    public GameObject thePlayer;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){
            thePlayer.transform.position = teleportTarget.transform.position; 
        }
        //else
        //{
        //    thePlayer.transform.position = teleportTarget.transform.position;
        //}
       
    }
    
}
