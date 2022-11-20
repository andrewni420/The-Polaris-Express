using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Enemy : MonoBehaviour, Damageable
{
    

    public AI playerPrediction;
    protected EnemyTrajectory trajectory;
    protected State state;
    protected float maxSpeed = 1F;
    protected int health = 100;
    protected int maxHealth = 100;
    protected int damage = 20;
    protected int knockback = 5;
    protected float hitCooldown = 0;
    public intelligence intLevel=intelligence.pos;
    protected Vector3 moveDirection;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected (float range, float angle, bool ignoreObstacles) FOV = (10f,90f,false);
    protected float renderDistance = 50f;
    // private bool isInAnimation = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        state = new State();
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
        

        Vector3[] nextMove = getNextMove(playerTrajectory);
        if (nextMove[0] != moveDirection)
        {
            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.destination = hitCooldown == 0 ? nextMove[0] : transform.position;
            moveDirection = agent.destination;
        }

        hitCooldown = Math.Max(hitCooldown - Time.fixedDeltaTime, 0);
        
        transform.Rotate(nextMove[1]);
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (distance > 50) Destroy(gameObject);

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
    

    public abstract Vector3[] getNextMove(Vector3[] position);
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "playerWeapon")
        {
            onHit(other.gameObject);
        }
            
            // in future, access damage from other
            // hitAnimation();
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
    }

    // public void hitAnimation()
    // {
    //     isInAnimation = true;
    // }
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
            Destroy(gameObject);
        }
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
        Vector3 dir = (transform.position - player.transform.position).normalized;
        GetComponent<Rigidbody>().AddForce((dir * player.getKnockback() + transform.up * 3).normalized*player.getKnockback(), ForceMode.Impulse);
    }
    public bool canSee(Vector3 other)
    {
        float angle = Vector3.Angle(transform.forward, other);
        Vector3 dir = (other - transform.position);
        float distance = dir.magnitude;
        if (distance > FOV.range || angle > FOV.angle) return false;
        if (FOV.ignoreObstacles) return true;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, FOV.range))
        {
            if (hit.collider.tag != "Player") return false;
        }

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
    
}
