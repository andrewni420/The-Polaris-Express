using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaredEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        Vector3 nextMove = (2*transform.position - playerTrajectory[0]).normalized * trajectory.getMaxSpeed() * Time.fixedDeltaTime;
        state.addState("escaping");
        return new Vector3[] { nextMove, new Vector3(0, 0, 0) };
    }
}
