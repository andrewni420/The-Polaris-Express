using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEnterCave : MonoBehaviour
{
    //Used https://www.youtube.com/watch?v=E7-HAJ4Db64 as a tutorial for this code

    Collider soundTrigger;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider soundCollider)
    {
        GetComponent<AudioSource>().Play();
    }

    void OnTriggerExit(Collider soundCollider)
    {
        GetComponent<AudioSource>().Stop();
    }
}
