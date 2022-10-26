using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AggressiveEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {

        Vector3 nextMove = (playerTrajectory[0] - transform.position).normalized * trajectory.getMaxSpeed() * Time.fixedDeltaTime;
        state.addState("attacking");
        return new Vector3[] { nextMove, new Vector3(0, 0, 0) };
    }
}
