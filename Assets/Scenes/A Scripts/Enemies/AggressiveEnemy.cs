using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AggressiveEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        if ((transform.position - playerTrajectory[0]).magnitude > 10) return trajectory.getNextMove();

        float speed = GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        state.addState("attacking");
        Vector3 nextMove;
        switch (intLevel)
        {
            case intelligence.pos:
                nextMove = playerTrajectory[0];
                break;
            case intelligence.vel:
                nextMove = playerTrajectory[0] + playerTrajectory[1] * (transform.position - playerTrajectory[0]).magnitude / speed;
                break;
            default:
                nextMove = transform.position;
                break;
        }
        return new Vector3[] { nextMove, new Vector3(0, 0, 0) };
    }
}
