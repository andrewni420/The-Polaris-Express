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
    public int maxHealth = 10000;
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
    public ItemObject starlight;
    private float starlightTimer = 1f;
    //Should be low to encourage killing enemies
    private float starlightChance = 0.1f;

    public GameObject lightLight;

    public GameObject dayTime;
    private float curTime;
    private float lastTimeSunDamage;
    private float runningTime;
    public int sunDamage;
    public float sunDamageInterval;
    public int caveHeal;
    public float caveHealInterval;
    private float caveHealTimer =0f;

    public int damage = 0;
    public int knockback = 0;

    private float invincibilityTimer = 0f;
    private float strengthTimer = 0f;
    private float lightTimer = 0;
    public Spawner spawner;

    public GameObject strengthIcon;
    public GameObject invincibilityIcon;
    public GameObject lightIcon;


    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        curHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);
        history.init(gameObject);
        inventory.Awake();
        lastTimeSunDamage = dayTime.GetComponent<DayNight>().currentTime;
    }

    void OnTriggerStay(Collider other)
    {
        //switch (other.gameObject.tag)
        //{
        //    case "Item":
        //        return;
        //}
        //OnTriggerEnter(other);
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
                Debug.Log("enemy");
                if (immunityTimer > 0) break;
                Enemy enemy = other.gameObject.GetComponent<AggressiveEnemy>();
                if (enemy == null) break;
                enemy.setHitCooldown(1f);
                onHit(enemy.getDamage());
                Vector3 dir = (transform.position - enemy.transform.position).normalized;
                GetComponent<Rigidbody>().AddForce((dir * enemy.getKnockback() + transform.up * 3).normalized*enemy.getKnockback(), ForceMode.Impulse);
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

    void onCollisionEnter(Collision other)
    {
        Debug.Log(other);
        if (other.collider.tag == "Enemy")
        {
            if (immunityTimer > 0) return;
            Enemy enemy = other.gameObject.GetComponent<AggressiveEnemy>();
            Debug.Log(enemy);
            if (enemy == null) return;
            enemy.setHitCooldown(1f);
            onHit(enemy.getDamage());
            Vector3 dir = (transform.position - enemy.transform.position).normalized;
            GetComponent<Rigidbody>().AddForce(dir * enemy.getKnockback() + transform.up * 3, ForceMode.Impulse);
        }
    }
    
    void Update()
    {
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

        if ((curHealth == 0) || (curHunger == 0))
        {
            
            SceneManager.LoadScene("LoseMenu");     
        }

        if (gameManager.area != gameArea.cave) 
        {
            runningTime = Mathf.Floor((dayTime.GetComponent<DayNight>().translateTime) * 24f);
            curTime = runningTime % 24;

            if (curTime == 0)
            {
                lastTimeSunDamage = 0;
            }

            if ((0.0f < curTime) && (curTime < 8f))
            {
                if ((curTime == lastTimeSunDamage + sunDamageInterval) || (lastTimeSunDamage == 0))
                {
                    DamagePlayer(sunDamage);
                    lastTimeSunDamage = curTime;
                }
            }
        }
        else
        {
            if (caveHealTimer == 0)
            {
                caveHealTimer = caveHealInterval;
                HealPlayer(caveHeal);
            }
        }
        
        updateDamage();
        history.updateStats(curHunger, curHealth);

        if (starlightTimer == 0)
        {
            starlightTimer = 1f;
            if (UnityEngine.Random.Range(0f, 1f) < starlightChance) inventory.AddItem(starlight, 1);
        }

        updateIcons();  
    }

    public void updateIcons()
    {
        if (invincibilityTimer > 0) invincibilityIcon.SetActive(true);
        else invincibilityIcon.SetActive(false);
        
        if (strengthTimer > 0) strengthIcon.SetActive(true);
        else strengthIcon.SetActive(false);
        
        if (lightTimer > 0) 
        {
            lightIcon.SetActive(true);
            lightLight.SetActive(true);
            spawner.setSuppressed(true);
        } else {
            lightIcon.SetActive(false);
            lightLight.SetActive(false);
            spawner.setSuppressed(false);
        }

    }

    // Player takes damage, looses health points
    public void DamagePlayer( int damage )
    {
        if (invincibilityTimer > 0) return;
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
        invincibilityTimer = Math.Max(invincibilityTimer - time, 0);
        strengthTimer = Math.Max(strengthTimer - time, 0);
        lightTimer = Math.Max(lightTimer - time, 0);

        recentHits = recentHitTimer == 0 ? 0 : recentHits;
        starlightTimer = Math.Max(starlightTimer - time, 0);
        caveHealTimer = Math.Max(caveHealTimer - time, 0);
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

    public void activateBuff(string buff)
    {
        switch (buff)
        {
            case "Invincibility":
                invincibilityTimer = 20;
                break;
            case "Strength":
                strengthTimer = 15;
                break;
            case "Light":
                lightTimer = 5;
                break;
            default:
                break;
        }
    }

    // Player eats, gain hunger points
    public void Eat(int hunger)
    {
        curHunger += hunger;
        enforceHungerBounds();

        hungerBar.SetHunger(curHunger);
        Debug.Log("yum");
    }

    private void OnApplicationQuit()
    {
        inventory.Container.Clear();
        foreach (InventorySlot s in inventory.materials) s.amount = 0;
        inventory.numStars = 0;
    }

    private void updateDamage()
    {
        damage = inventory.getDamage();
        knockback = inventory.getKnockback();
    }
    
    public int getDamage()
    {
        if (strengthTimer > 0) return damage * 2;
        return damage;
    }

    public int getKnockback()
    {
        if (strengthTimer > 0) return (int)(knockback*1.25f);
        return knockback;
    }

}
