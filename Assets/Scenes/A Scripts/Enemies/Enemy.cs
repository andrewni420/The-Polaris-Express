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
    private bool isInAnimation = false;

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


    void FixedUpdate()
    {
        trajectory.update();

        Vector3[] playerTrajectory = enemyAI.imputeTrajectory();
        

        Vector3[] nextMove = getNextMove(playerTrajectory);


        transform.position += nextMove[0];
        transform.Rotate(nextMove[1]);

        ////////////Kelly edit here///////////
        ///
        float random = UnityEngine.Random.Range(0F, 100F);
        if (random < 25)
        {
            float hi = 1;
            ///25% chance each fixedUpdate to play hit animation. Adjust as necessary
        }
        
        

    }

    public int getHealth()
    {
        return health;
    }
    public void setHealth(int health)
    {
        this.health = health;
    }
    public void enforceMaxHealth()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    

    public abstract Vector3[] getNextMove(Vector3[] playerTrajectory);
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "playerWeapon")
        {
            onHit(30);
            hitAnimation();
        }
        
    }
    public void hitAnimation()
    {
        isInAnimation = true;
        

    }
    public void onHit(int damage)
    {
        takeDamage(damage);
    }
    public void takeDamage(int damage)
    {
        health -= damage;
    }
    
}
