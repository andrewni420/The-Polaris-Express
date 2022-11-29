using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


// Tutorial: https://www.youtube.com/watch?v=BLfNP4Sc_iA
// Stores player health and hunger and takes interactions that influence health and hunger


public class PlayerHealthView : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerHistory history;
    // Health variables 
    public int curHealth;
    public int maxHealth = 100;
    public HealthBar healthBar;

    // Hunger variables 
    public int curHunger;
    public int maxHunger = 100;

    public HungerBar hungerBar;
    float elapsed = 0f;

    private float immunityTimer = 0f;
    private float stunTimer = 0f;
    private float recentHitTimer = 0f;

    private int recentHits = 0;

    public Inventory inventory;

    public int damage = 0;
    public int knockback = 0;

    void Start()
    {


        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        curHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);
        history.init(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Item":
                return;
        }
        OnTriggerEnter(other);
    }

    void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag)
        {
            // When interacting with food, increase hunger points and destroy food object
            case "Food":
                other.gameObject.SetActive(false);
                Eat(5);
                break;
            // When interacting with enemy, decrement health points in hit
            case "Enemy":
                if (immunityTimer > 0) break;
                Enemy enemy = other.gameObject.GetComponent<AggressiveEnemy>();
                if (enemy == null) break;
                enemy.setHitCooldown(1f);
                onHit(enemy.getDamage());
                Vector3 dir = (transform.position - enemy.transform.position).normalized;
                GetComponent<Rigidbody>().AddForce(dir * enemy.getKnockback() + transform.up*3, ForceMode.Impulse);
                break;
            case "Item":
                var item = other.GetComponent<Item>();
                if (item)
                {
                    inventory.AddItem(item.getItem(), item.getAmount());
                    Destroy(other.gameObject);
                }
                break;
        }
		}
    
    void Update()
    {
      
        // Place holder functionality to show the health bar movement  
        if( Input.GetKeyDown( KeyCode.F) )
        {
            DamagePlayer(5);
            Debug.Log("F key was pressed.");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            HealPlayer(5);
            Debug.Log("H key was pressed.");
        }
        if (Input.GetKey(KeyCode.E))
        {
            inventory.craftSword();
        }

        // Have hunger bar decrease in  value over time
        elapsed += Time.deltaTime;
        if (elapsed >= 2) 
          {
              elapsed = elapsed % 2;
              curHunger -= 1;
              enforceHungerBounds();
              hungerBar.SetHunger(curHunger);
          }

        updateTimers(Time.deltaTime);

        if ((curHealth == 0) || (curHunger == 0)){
            
            SceneManager.LoadScene("LoseMenu");     
          }
        updateDamage();
        history.updateStats(curHunger, curHealth);
    }

    // Player takes damage, looses health points
    public void DamagePlayer( int damage )
    {
        curHealth -= damage;
        enforceHealthBounds();

        healthBar.SetHealth(curHealth);
    }

    // Player heals, gains health points
    public void HealPlayer( int damage )
    {
        curHealth += damage;
        enforceHealthBounds();

        healthBar.SetHealth(curHealth);
    }

    // Player takes damage from a hit
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

    // Player eats, gain hunger points
    public void Eat( int hunger )
    {
        curHunger += hunger;
        enforceHungerBounds();

        hungerBar.SetHunger(curHunger);
        Debug.Log("yum");
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
    }
    private void updateDamage()
    {
        if (inventory.hasItem("Sword"))
        {
            damage = 100;
            knockback = 10;
        }
        else if (inventory.hasItem("Dagger"))
        {
            damage = 10;
            knockback = 5;
        }
        else
        {
            damage = 1;
            knockback = 2;
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

}
