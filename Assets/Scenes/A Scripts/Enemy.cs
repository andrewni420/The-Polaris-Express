using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AI enemyAI;
    public Health enemyHealth;
    private float maxSpeed = 1F;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerHurtbox")
        {

        }
    }

    void FixedUpdate()
    {
        Vector3[] playerTrajectory = enemyAI.imputeTrajectory();
        transform.position = Vector3.MoveTowards(transform.position, playerTrajectory[0], maxSpeed*Time.fixedDeltaTime);

    }
}
