using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthView : MonoBehaviour
{
    public int curHealth;
    public int maxHealth = 100;

    public int curHunger;
    public int maxHunger = 100;

    public HealthBar healthBar;
    public HungerBar hungerBar;
    float elapsed = 0f;


    private float immunityTimer = 0f;
    private float stunTimer = 0f;
    private float recentHitTimer = 0f;

    private int recentHits = 0;

    void Start()
    {
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        curHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);

    }

    void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag)
        {
            case "Food":
                other.gameObject.SetActive(false);
                Eat(5);
                break;
            case "Enemy":
                if (immunityTimer > 0) break;
                Enemy enemy = other.gameObject.GetComponent<AggressiveEnemy>();
                if (enemy == null) break;
                enemy.setHitCooldown(1f);
                onHit(enemy.getDamage());
                Vector3 dir = (transform.position - enemy.transform.position).normalized;
                GetComponent<Rigidbody>().AddForce(dir * enemy.getKnockback() + transform.up*3, ForceMode.Impulse);
                Debug.Log(dir*enemy.getKnockback());
                break;
        }
		}
    
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.F) )
        {
            DamagePlayer(5);
            Debug.Log("F key was pressed.");
        }

        if( Input.GetKeyDown( KeyCode.H) )
        {
            HealPlayer(5);
            Debug.Log("H key was pressed.");
        }
        

        elapsed += Time.deltaTime;
        if (elapsed >= 2) 
          {
              elapsed = elapsed % 2;
              curHunger -= 1;
              enforceHungerBounds();
              hungerBar.SetHunger(curHunger);
          }

        updateTimers(Time.deltaTime);
    }

    public void DamagePlayer( int damage )
    {
        curHealth -= damage;
        enforceHealthBounds();

        healthBar.SetHealth(curHealth);
    }

    public void HealPlayer( int damage )
    {
        curHealth += damage;
        enforceHealthBounds();

        healthBar.SetHealth(curHealth);
    }

    public void onHit(int damage)
    {

        DamagePlayer(damage);
        recentHits += 1;

        //Timers in seconds
        stunTimer = 3f / (recentHits + 1);
        recentHitTimer = 5f;
        immunityTimer = 0.5f;

        //Backward jump upon hit
    }
    public void updateTimers(float time)
    {
        stunTimer = Math.Max(stunTimer - time, 0);
        recentHitTimer = Math.Max(recentHitTimer - time, 0);
        immunityTimer = Math.Max(immunityTimer - time, 0);

        recentHits = recentHitTimer == 0 ? 0 : recentHits;
    }
    //Ensure health/hunger is always between 0 and maxHealth/maxHunger
    public int enforceHealthBounds()
    {
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
            return 1;
        }
        if (curHealth < 0)
        {
            curHealth = 0;
            return -1;
        }
        return 0;
    }
    public int enforceHungerBounds()
    {
        if (curHunger > maxHunger)
        {
            curHunger = maxHunger;
            return 1;
        }
        if (curHunger < 0)
        {
            curHunger = 0;
            return -1;
        }
        return 0;
    }


    public void Eat( int hunger )
    {
        curHunger += hunger;
        enforceHungerBounds();

        hungerBar.SetHunger(curHunger);
        Debug.Log("yum");
    }


}
