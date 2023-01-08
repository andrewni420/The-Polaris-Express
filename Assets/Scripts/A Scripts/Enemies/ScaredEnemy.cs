using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaredEnemy : PassiveEnemy
{
    private float deAggroDistance = 40f;

    public override void updateState(Vector3[] playerTrajectory)
    {
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (canSee(playerTrajectory[0]))
        {
            state = enemyState.scared;
        }
        else if (distance > deAggroDistance)
        {
            state = enemyState.passive;
        }
        
    }
}
