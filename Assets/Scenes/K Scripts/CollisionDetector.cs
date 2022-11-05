using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// resource: https://www.youtube.com/watch?v=aNZw588BQBo
// Zyger
public class CollisionDetector : MonoBehaviour
{
    public WeaponControls wc;
    public GameObject player;
    // public GameObject HitParticle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && wc.isAttacking) 
        {
            //Debug.Log(other.name);
            other.GetComponent<Animator>().SetTrigger("Hit");
            // Instantiate(HitParticle, new Vector3(other.transform.position.x,
            // transform.position.y, other.transform.position.z), other.transform.rotation);
        }
    }
    public GameObject getPlayer()
    {
        return player;
    }
}
