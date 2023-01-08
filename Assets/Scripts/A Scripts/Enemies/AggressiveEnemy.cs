using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AggressiveEnemy : PassiveEnemy
{
    private float deAggroDistance = 20;

    public override void updateState(Vector3[] playerTrajectory)
    {
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (canSee(playerTrajectory[0]))
        {
            state = enemyState.aggressive;
        }
        else if (distance > deAggroDistance)
        {
            state = enemyState.passive;
        }
    }
}
