using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public enum enemyState
{
    aggressive, passive, scared, none
}

public abstract class Enemy : MonoBehaviour
{
    

    public AI playerPrediction;
    protected EnemyTrajectory trajectory;
    protected float maxSpeed = 1F;
    public float maxRotSpeed = 1.0F;
    protected int health = 100;
    protected int maxHealth = 100;
    public int damage = 20;
    public int knockback = 5;
    protected float hitCooldown = 0;
    public intelligence intLevel=intelligence.pos;
    protected Vector3 moveDirection;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected (float range, float angle) FOV = (20f,90f);
    protected float renderDistance = 100f;
    protected enemyState state = enemyState.none;
    protected EnemyMovement movement;
    public PlayerHealthView playerStats;
    public Loot[] lootTable;
    private Rigidbody rigidBody;
    private bool movementSuppressed;
    public Spawner spawner;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        movement = new EnemyMovement(gameObject);
        trajectory = new EnemyTrajectory(this.gameObject,maxSpeed);
        moveDirection = transform.position;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  
    void FixedUpdate()
    {
        trajectory.update();
        Vector3[] playerTrajectory = playerPrediction.imputeMotion();
        updateState(playerTrajectory);

 
        (Vector3 dir,Vector3 rot) nextMove = movement.getNextMove(state,playerTrajectory);
        Vector3 dir;
        if (spawner.projectNavMesh(nextMove.dir, out dir))
        {
            if (dir != moveDirection)
            {
                agent.destination = hitCooldown == 0 ? dir : transform.position;
                moveDirection = agent.destination;
            }
        }
        

        hitCooldown = Math.Max(hitCooldown - Time.fixedDeltaTime, 0);

        transform.Rotate(nextMove.rot);
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (distance > renderDistance)
        {
            Destroy(gameObject);
        }
        CorrectBaseHeight();


        if (movementSuppressed)
        {
            agent.destination = transform.position;
            moveDirection = transform.position;
        }
    }

    public void setSuppressed(bool s) { movementSuppressed = s; }

    private void CorrectBaseHeight()
    {
        NavMeshHit navhit;
        if (NavMesh.SamplePosition(transform.position, out navhit, 4f, NavMesh.AllAreas))
        {
            Ray r = new Ray(navhit.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 5f, LayerMask.GetMask("ground")))
            {
                agent.baseOffset = -hit.distance;
            }
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
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "playerWeapon")
        {
            onHit(other.gameObject);
        }
            
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.tag == "Ground")
        {
            Vector3 position = transform.position;
            if (agent.enabled)
            {
                
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.isStopped = false;
                agent.Warp(position);
            }
        }
        //if (collision.collider.tag == "Player")
        //{
        //    collision.gameObject.GetComponent
        //}
    }

    public void setHitCooldown(float time)
    {
        hitCooldown = time;
    }
    public void takeDamage(int damage)
    {
        health -= damage;
        checkHealth();
    }
    public void checkHealth()
    {
        if (health <= 0)
        {

            onDeath();
            
        }
    }

    public void onDeath()
    {
        foreach (Loot l in lootTable)
        {
            if (UnityEngine.Random.Range(0f, 1f) < l.chance) playerStats.inventory.AddItem(l.item, l.amount);
        }
        Destroy(gameObject);
        
    }

    public int getDamage()
    {
        return damage;
    }
    public int getKnockback()
    {
        return knockback;
    }
    public void onHit(GameObject other)
    {
        if (agent.enabled)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.isStopped = true;
        }
        CollisionDetector collisionDetector = other.GetComponent<CollisionDetector>();
        PlayerHealthView player = collisionDetector.getPlayer().GetComponent<PlayerHealthView>();
        if (player == null) return;
        takeDamage(player.getDamage());
        Vector3 dir = (transform.position - player.transform.position);
        dir = new Vector3(dir.x, 0, dir.z);
        dir = dir.normalized;
        rigidBody.AddForce((dir * player.getKnockback() + Vector3.up * 3).normalized * player.getKnockback(), ForceMode.Impulse);
        Debug.Log((dir * player.getKnockback() + Vector3.up * 3).normalized * player.getKnockback());
    }
    public bool canSee(Vector3 other)
    {
        Vector3 dir = other - transform.position;

        float angle = Vector3.Angle(transform.forward, dir);
        float distance = dir.magnitude;
        if (distance > FOV.range || angle > FOV.angle) return false;
        return true;


    }
    public bool canSee(GameObject other)
    {
        return canSee(other.transform.position);
    }
    public void setStats(int maxHealth, int health, int damage, int knockback)
    {
        this.maxHealth = maxHealth;
        this.health = health;
        this.damage = damage;
        this.knockback = knockback;
    }
    public void setInt(intelligence intLevel)
    {
        this.intLevel = intLevel;
    }
    public abstract void updateState(Vector3[] playerTrajectory);
    
}

[System.Serializable]
public class Loot
{
    public ItemObject item;
    public int amount;
    public float chance;
}
