using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthView : MonoBehaviour
{

    public int curHealth;
    public int maxHealth = 100;

    public HealthBar healthBar;

    
    void Start()
    {
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "playerWeapon")
        {
            takeDamage(10);    
        }        
    }
  
    public void takeDamage(int damage)
    {
        curHealth -= damage;

        healthBar.SetHealth(curHealth);
    }
}
