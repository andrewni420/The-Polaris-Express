using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AggressiveEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //Vector3 nextMove = (playerTrajectory[0] - transform.position).normalized * trajectory.getMaxSpeed() * Time.fixedDeltaTime;
        state.addState("attacking");
        Vector3 nextMove = playerTrajectory[0] + playerTrajectory[1] * (transform.position - playerTrajectory[0]).magnitude / agent.speed;
        //return new Vector3[] { nextMove, new Vector3(0, 0, 0) };
        return new Vector3[] { playerTrajectory[0], new Vector3(0, 0, 0) };
    }
}
