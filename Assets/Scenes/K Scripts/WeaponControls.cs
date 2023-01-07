using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference/resource -- edited a bit to work for project
// https://www.youtube.com/watch?v=aNZw588BQBo
// Zyger
public class WeaponControls : MonoBehaviour
{
    
    [Header("Fighting Properties")]
    public GameObject weapon;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    // can also put in an audio for a sword attack
    public bool isAttacking = false;

    [Header("Access Points Needed")]
    public Inventory inventory;
    public GameObject thePlayer;
    public WeaponControls weaponHolder;

    [Header("Possible Weapons")]
    public GameObject sword;
    public GameObject dagger;
    public GameObject cylinder;

    public void Start()
    {
        Debug.Log("Weapon changed to Cylinder");
        weapon = Instantiate(cylinder, new Vector3(((transform.position.x)-3),((transform.position.y)), ((transform.position.z))-2), Quaternion.identity);
        weapon.transform.parent = gameObject.transform;
        // weapon = Instantiate(cylinder);
        // makeChanges(weapon);
        weapon.GetComponent<CollisionDetector>().wc = weaponHolder;
        weapon.GetComponent<CollisionDetector>().player = thePlayer;
    }

    public void FixedUpdate()
    {
        ItemObject weaponObject = inventory.Container[inventory.getSelection()].item;        

        if (weaponObject != null)
        {
            string weaponName = weaponObject.itemName;
            Debug.Log(weaponName + " is the item selected");
            string currentWeapon = weapon.name;
            
            // if ((weaponName.Equals("Sword")) && (!(currentWeapon.Equals("Sword(Clone)"))))
            // {
            //     Debug.Log("Weapon changed to Sword");
            //     weapon = Instantiate(sword, new Vector3(transform.position.x,transform.position.y, transform.position.z), Quaternion.identity);
            //     weapon.transform.parent = gameObject.transform;
            //     weapon.GetComponent<CollisionDetector>().wc = weaponHolder;
            //     weapon.GetComponent<CollisionDetector>().player = thePlayer;
            // }
            
            // if ((weaponName.Equals("Dagger")) && (!(currentWeapon.Equals("Dagger(Clone)"))))
            // {
            //     Debug.Log("Weapon changed to Dagger");
            //     weapon = Instantiate(dagger, new Vector3(transform.position.x,transform.position.y, transform.position.z), Quaternion.identity);
            //     weapon.transform.parent = gameObject.transform;
            //     // makeChanges(weapon);
            //     weapon.GetComponent<CollisionDetector>().wc = weaponHolder;
            //     weapon.GetComponent<CollisionDetector>().player = thePlayer;
            // }
            if ((weaponName.Equals("Sword")) || (weaponName.Equals("Dagger")))
            {
                switch (weaponName)
                {
                    case "Sword":
                        if (!currentWeapon.Equals("Sword(Clone)"))
                        {
                            Destroy(weapon);
                            weapon = Instantiate(sword, new Vector3(transform.position.x,transform.position.y, transform.position.z), Quaternion.identity);
                            weapon.transform.parent = gameObject.transform;
                            weapon.GetComponent<CollisionDetector>().wc = weaponHolder;
                            weapon.GetComponent<CollisionDetector>().player = thePlayer;
                            Debug.Log("Weapon changed to Sword");
                        }
                        // makeChanges(weapon);
                        break;
                    
                    case "Dagger":
                        if (currentWeapon.Equals("Dagger(Clone)"))              
                        {
                            Destroy(weapon);
                            Debug.Log("Weapon changed to Dagger");
                            weapon = Instantiate(dagger, new Vector3(transform.position.x,transform.position.y, transform.position.z), Quaternion.identity);
                            weapon.transform.parent = gameObject.transform;
                            // makeChanges(weapon);
                            weapon.GetComponent<CollisionDetector>().wc = weaponHolder;
                            weapon.GetComponent<CollisionDetector>().player = thePlayer;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            if (CanAttack)
            {
                Debug.Log("Weapon Swung");
                attack();
            }
        }
    }

    public void attack()
    {
        isAttacking = true;
        CanAttack = false;
        Animator anim = weapon.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        // play the sound here if decide to use it
        Invoke(nameof(ResetIsAttacking), AttackCooldown);
        Invoke(nameof(ResetAttack), AttackCooldown);
    }

    public void makeChanges(GameObject weaponToUpdate)
    {
        weaponToUpdate.GetComponent<CollisionDetector>().wc = weaponHolder;
        weaponToUpdate.GetComponent<CollisionDetector>().player = thePlayer;
    }

    public void ResetAttack()
    {
        CanAttack = true;
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

}
