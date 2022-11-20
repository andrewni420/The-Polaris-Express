using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrajectory : Trajectory
{
    protected Queue<Vector3> predTraj;
    protected Queue<float> predAVel;
    protected float maxSpeed = 1.0F;
    protected float maxRotSpeed = 1.0F;

    public EnemyTrajectory(GameObject entity, float maxSpeed) : base(entity)
    {
        
        predTraj = new Queue<Vector3>();
        predAVel = new Queue<float>();
        this.maxSpeed = maxSpeed;
    }


    
    public Queue<Vector3> getPredTraj()
    {
        return predTraj;
    }
    public Vector3 popPredTraj()
    {
        return predTraj.Dequeue();
    }
    public Queue<float> getPredAVel()
    {
        return predAVel;
    }
    public float popPredAVel()
    {
        return predAVel.Dequeue();
    }

    public void addForwardTraj()
    {
        int moveLength = UnityEngine.Random.Range(60, 100);
        int moveDist = UnityEngine.Random.Range(10, 30);
        Vector3 forward = entity.transform.position + entity.transform.forward * moveDist;
        for (int i = 0; i < moveLength; i++)
        {
            predTraj.Enqueue(forward);
            predAVel.Enqueue(0);
        }
    }
    public void addRotationTraj()
    {
        int rotLength = UnityEngine.Random.Range(20, 60);
        float leftRight = 2 * UnityEngine.Random.Range(0, 2) - 1;
        float rotSpeed = UnityEngine.Random.Range(0.8F, 1F) * maxRotSpeed * leftRight;
        for (int i = 0; i < rotLength; i++)
        {
            predAVel.Enqueue(rotSpeed);
            predTraj.Enqueue(entity.transform.position);
        }
    }

    public void addWaitTraj()
    {
        int waitLength = UnityEngine.Random.Range(20, 40);
        for (int i = 0; i < waitLength; i++)
        {
            predTraj.Enqueue(entity.transform.position);
            predAVel.Enqueue(0);
        }
    }
    public void predictMovement()
    {
        //Movement types: move forward / rotate
        float moveType = UnityEngine.Random.Range(0F, 1F);

        if (moveType < 0.33) addForwardTraj();
        else if (moveType < 0.66) addRotationTraj();
        else addWaitTraj();
    }

    public Vector3[] getNextMove()
    {

        if (predTraj.Count < 5) predictMovement();
        Vector3 nextTraj = popPredTraj();
        float nextAngle = popPredAVel();

        return new Vector3[] { nextTraj, new Vector3(0, nextAngle, 0) };
    }

    public float getMaxSpeed()
    {
        return maxSpeed;
    }
}
