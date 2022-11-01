using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthView : MonoBehaviour
{

    public HealthBar healthBar;
    public Enemy enemy;
    public  FPCam cam;

 
    
    void Start()
    {

        healthBar.SetMaxHealth(enemy.getHealth());

        

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

            transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1, enemy.transform.position.z);
            transform.rotation = cam.orientation.rotation;
        }
    }
}
