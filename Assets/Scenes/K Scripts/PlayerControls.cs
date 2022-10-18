using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControls : MonoBehaviour {

    public Vector2 moveValue;
    public float speed;

    public int curHealth;
    public int maxHealth = 100;

    public HealthBar healthBar;

    void Start()
    {
        curHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void FixedUpdate() {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime); 

        if( Input.GetKeyDown( KeyCode.D ) )
        {
            DamagePlayer(5);
            Debug.Log("D key was pressed.");
        }

        if( Input.GetKeyDown( KeyCode.H) )
        {
            HealPlayer(5);
            Debug.Log("H key was pressed.");
        }
    }

    void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
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