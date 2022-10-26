using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference/resource -- edited a bit to work for project
// https://www.youtube.com/watch?v=aNZw588BQBo
// Zyger
public class WeaponControls : MonoBehaviour
{
    public GameObject Sword;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    // can also put in an audio for a sword attack
    public bool isAttacking = false;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (CanAttack)
            {
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
        // this is were we should talk to Andy -- enemy interaction
        // w/ health & everything
        isAttacking = true;
        CanAttack = false;
        Animator anim = Sword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        // play the sound here if decide to use it
        Invoke(nameof(ResetIsAttacking), AttackCooldown);
        Invoke(nameof(ResetAttack), AttackCooldown);
    }

    public void ResetAttack()
    {
        // ResetIsAttacking();
        CanAttack = true;
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

}
