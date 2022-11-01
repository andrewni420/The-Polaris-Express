using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaredEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        //if ((transform.position - playerTrajectory[0]).magnitude > 20) return trajectory.getNextMove();

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
