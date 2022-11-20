using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaredEnemy : PassiveEnemy
{
    private bool scared = false;
    private float deAggroDistance = 10f;
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        float distance = (transform.position - playerTrajectory[0]).magnitude;
        if (canSee(playerTrajectory[0]))
        {
            scared = true;
        }
        else if (distance > deAggroDistance)
        {
            scared = false;
        }

        if (!scared) return base.getNextMove(playerTrajectory);

        Vector3 nextMove;
        switch (intLevel)
        {
            case intelligence.pos:
            case intelligence.vel:
                nextMove = transform.position+(transform.position - playerTrajectory[0]).normalized;
                break;
            default:
                nextMove = transform.position;
                break;
        }
        
        state.addState("escaping");
        return new Vector3[] { nextMove, new Vector3(0, 0, 0) };
    }
}
