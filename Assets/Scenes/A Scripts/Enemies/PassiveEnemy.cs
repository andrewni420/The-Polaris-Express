using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : Enemy
{
    public override Vector3[] getNextMove(Vector3[] playerTrajectory)
    {
        //return actionByState(playerTrajectory);
        return trajectory.getNextMove();

    }
    /*
    //Stand for 4s, walk for 2s with 1 slight direction change, TURN IN PLACE, repeat. 
    public Vector3 actionByState(Vector3[] playerTrajectory)
    {
        Tuple<string, float> curState = state.getCurState();
        switch (curState.Item1)
        {
            case "wander":
                return wanderAction(playerTrajectory);
            case "dirChange":
                return dirChangeAction(playerTrajectory);
            case "maintain":
                return maintainAction(playerTrajectory);
            default:
                return standAction(playerTrajectory);
        }
    }

    //Stand for 2s on average
    private Vector3 standAction(Vector3[] playerTrajectory)//playerTrajectory is a list of trajectories for n future fixedUpdate intervals
    {
        float nextAction = UnityEngine.Random.Range(0F, 1F);
        if (nextAction < 0.975)
        {
            return stand(playerTrajectory);
        }
        else
        {
            return dirChange(playerTrajectory);
        }
    }

    //dirChange is a rotation in place -> more dir change or wandering
    //The issue is that we're not moving and Enemy.cs expects us to move.
    //Return a tuple - direction and max speed -> allows for different speeds (running away vs passive moving vs chasing/combat...?)
    //To smooth motions, need to track state durations -
	    //"stand" -> "speedup" -> "maintain" -> "dirChange(randomAngle10)" -> "slowdown" -> "stand" -> "dirChange(randomAngle180)"
        //Use transform.rotate and transform.forward to do local coordinates -> makes a good case for returning tuples -> have to use max() to not overshoot
    //How to model inertia?
        //add force until max speed, add force to turn, add force to come to stop
    //Create levels of enemy AI - stand still / trajectory + player trajectory / + last state identity / + last state duration / multiple states
    //Because the AI doesn't know what it's about to do, it can't use lerping
        //How to include future intentions into AI? List of future states. Random chance of changing their mind. Environmental inputs also change their mind
    private Vector3 dirChangeAction(Vector3[] playerTrajectory)
    {
        return wander(playerTrajectory);
    }

    //go back to maintain after 0.2s -> 1/10 chance
    //If not maintaining, 10% chance to dirChange
    //This would probably be where I first implement probability decay
    private Vector3 wanderAction(Vector3[] playerTrajectory)
    {
        float nextAction = UnityEngine.Random.Range(0F, 1F);

        if (nextAction < 0.1)
        {
            return maintain(playerTrajectory);

        }
        else
        {
            return wander(playerTrajectory);
        }
    }

    //maintain for 2s -> 0.999 chance to maintain
    private Vector3 maintainAction(Vector3[] playerTrajectory)
    {
        float nextAction = UnityEngine.Random.Range(0F, 1F);
        if (nextAction < 0.97)
        {
            return maintain(playerTrajectory);
        } else if (nextAction < 0.98)
        {
            return stand(playerTrajectory);
        }
        else
        {
            return wander(playerTrajectory);
        }
    }

    //Keeps current direction
    
    private Vector3 maintain(Vector3[] playerTrajectory)
    {
        state.addState("maintain");
        return trajectory[0] + trajectory[1];
    }

    //Changes to a random direction
    private Vector3 dirChange(Vector3[] playerTrajectory)
    {
        state.addState("dirChange");

        float randomAngle = (float)(UnityEngine.Random.Range(0F, 360F) * Math.PI / 180);
        Vector3 direction = new Vector3((float)(50 * Math.Cos(randomAngle)), 0, (float)(50 * Math.Sin(randomAngle)));
        return trajectory[0] + direction;
    }

    //Stands still
    private Vector3 stand(Vector3[] playerTrajectory)
    {
        state.addState("stand");
        return trajectory[0];
    }

    //Moves in direction close to current direction
    private Vector3 wander(Vector3[] playerTrajectory)
    {
        state.addState("wander");

        float angle = Vector3.Angle(new Vector3(1, 0, 0), trajectory[1]);
        float randomAngle = (float)(UnityEngine.Random.Range(angle - 10F, angle + 10F) * Math.PI / 180);
        Vector3 direction = new Vector3((float)(50 * Math.Cos(randomAngle)), 0, (float)(50 * Math.Sin(randomAngle)));
        return trajectory[0] + direction;
    }


    //random idle number -> idle(int number) -> skip / turn / wait / eat 
    //random vector to go towards
    //AI must avoid trees and player
    //Random number of random small deviations
    */
}
