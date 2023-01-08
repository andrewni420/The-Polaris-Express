using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : Enemy
{

    public override void updateState(Vector3[] playerTrajectory)
    {
        state = enemyState.passive;
    }

}
