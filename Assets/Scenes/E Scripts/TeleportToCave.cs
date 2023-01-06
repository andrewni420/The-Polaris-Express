using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToCave : MonoBehaviour
{
    public Transform TargetTransform;
    public Vector3 teleportTarget;
    public GameObject thePlayer;
    public bool suppressTeleport;

    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Player"){
            if (!suppressTeleport)
            {
                if (TargetTransform) teleportTarget = TargetTransform.position;
                thePlayer.transform.position = teleportTarget;
            }
        }
        //else
        //{
        //    thePlayer.transform.position = teleportTarget.transform.position;
        //}
       
    }

    void setSuppressed(bool s) { suppressTeleport = s; }
    
}
