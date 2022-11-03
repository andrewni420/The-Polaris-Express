using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Tutorial: https://www.youtube.com/watch?v=BLfNP4Sc_iA
// Initializes the enemy health bar and moves to towards the camera

public class EnemyHealthView : MonoBehaviour
{

    public HealthBar healthBar;
    public GameObject enemyObject;
    private Enemy enemy;
    public FPCam cam; 
    
    void Start()
    {
        enemy = enemyObject.GetComponent<Enemy>();
        healthBar.SetMaxHealth(enemy.getHealth());
        //Intiializes it from enemy class to 100 (full)
    }
    
    void Update(){
        if (enemy == null)
        {
            Destroy(healthBar.gameObject);
            Destroy(gameObject);
        }
        else
        {
            healthBar.SetHealth(enemy.getHealth());

            // Transform rotation to face camera 
            transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1, enemy.transform.position.z);
            transform.rotation = cam.orientation.rotation;
        }
    }
}
