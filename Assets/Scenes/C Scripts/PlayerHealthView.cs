using System.Collections;
using System.Collections.Generic;
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
    
    void Start()
    {
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        curHunger = maxHunger;
        hungerBar.SetMaxHunger(maxHunger);

    }

    void OnTriggerEnter(Collider other) { 
    	if(other.gameObject.tag == "Food") {
			other.gameObject.SetActive(false); 
			Eat(5);
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
              hungerBar.SetHunger(curHunger);
          }

    }

    public void DamagePlayer( int damage )
    {
        curHealth -= damage;

        healthBar.SetHealth(curHealth);
    }

    public void HealPlayer( int damage )
    {
        curHealth += damage;

        healthBar.SetHealth(curHealth);
    }

    public void Eat( int hunger )
    {
        curHunger += hunger;

        hungerBar.SetHunger(curHunger);
        Debug.Log("yum");
    }


}
