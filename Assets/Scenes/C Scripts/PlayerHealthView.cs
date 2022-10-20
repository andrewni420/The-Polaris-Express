using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthView : MonoBehaviour
{

    public int curHealth;
    public int maxHealth = 100;

    public HealthBar healthBar;
    
    void Start()
    {
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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
}
