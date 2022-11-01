using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trajectory
{
    private GameObject entity;
    private Vector3[] trajectory;
    private float angularVel = 0;
    private Vector3 forward;
    private Queue<Vector3> predTraj;
    private Queue<float> predAVel;
    private float maxSpeed = 1.0F;
    private float maxRotSpeed = 1.0F;


    public Trajectory(GameObject entity, float maxSpeed)
    {
        Transform transform = entity.transform;

        this.entity = entity;
        trajectory = new Vector3[] { transform.position, new Vector3(), new Vector3() };
        
        forward = transform.forward;
        predTraj = new Queue<Vector3>();
        predAVel = new Queue<float>();
        this.maxSpeed = maxSpeed;
    }

    public Vector3[] getTrajectory()
    {
        return trajectory;
    }
    public float getAVel()
    {
        return angularVel;
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

    public void addWaitTraj()
    {
        int waitLength = UnityEngine.Random.Range(20, 40);
        for (int i = 0; i < waitLength; i++)
        {
            predTraj.Enqueue(entity.transform.position);
            predAVel.Enqueue(0);
        }
    }
    public void addForwardTraj()
    {
        int moveLength = UnityEngine.Random.Range(60, 100);
        int moveDist = UnityEngine.Random.Range(10, 30);
        Vector3 forward = entity.transform.position+entity.transform.forward*moveDist;
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
        float rotSpeed = UnityEngine.Random.Range(0.8F,1F)*maxRotSpeed*leftRight;
        for (int i = 0; i < rotLength; i++)
        {
            predAVel.Enqueue(rotSpeed);
            predTraj.Enqueue(entity.transform.position);
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

    public void update()
    {
        trajectory = calcTrajectory(entity.transform.position);
        angularVel = calcAVel(entity.transform.forward);
    }

    private Vector3[] calcTrajectory(Vector3 pos)
    {
        Vector3 vel = (pos - trajectory[0]) / Time.fixedDeltaTime;
        Vector3 acc = (vel - trajectory[1]) / Time.fixedDeltaTime;
        return new Vector3[] { pos, vel, acc };
    }

    private float calcAVel(Vector3 dir)
    {
        return Vector3.Angle(dir, forward);
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


    //Rotate around transform.position, axis Vector3, __ degrees
    //Calculate vector3 angular velocity with magnitude in radians
}
