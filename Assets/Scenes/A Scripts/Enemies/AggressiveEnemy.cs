using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AggressiveEnemy : PassiveEnemy
{
    private bool aggressive = false;
    private float deAggroDistance = 10f;

    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (canSee(playerTrajectory[0]))
        {
            aggressive = true;
        } else if (distance > deAggroDistance)
        {
            aggressive = false;
        }

        if (!aggressive) return base.getNextMove(playerTrajectory);

        float speed = GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
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
