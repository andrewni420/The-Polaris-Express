using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public AI enemyAI;
    protected Trajectory trajectory;
    protected State state;
    private float maxSpeed = 1F;
    private int health = 100;
    private int maxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        state = new State();
        trajectory = new Trajectory(this.gameObject,maxSpeed);
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
        trajectory.update();

        Vector3[] playerTrajectory = enemyAI.imputeTrajectory();
        

        Vector3[] nextMove = getNextMove(playerTrajectory);


        transform.position += nextMove[0];
        transform.Rotate(nextMove[1]);
        

    }

    public int getHealth()
    {
        return health;
    }
    public void enforceMaxHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    

    public abstract Vector3[] getNextMove(Vector3[] playerTrajectory);
    public void onHit(int damage, GameObject player)
    {
        takeDamage(damage);
    }
    public void takeDamage(int damage)
    {
        health -= damage;
    }
    
}
