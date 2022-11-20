using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Trajectory
{
    protected GameObject entity;
    protected List<Vector3[]> trajectory;
    protected int maxPoints = 100;
    protected List<float> angularVel;
    protected Vector3 forward;



    public Trajectory(GameObject entity)
    {
        Transform transform = entity.transform;
        forward = transform.forward;
        this.entity = entity;
        trajectory = new List<Vector3[]>();
        trajectory.Add(new Vector3[] {transform.position, new Vector3(), new Vector3()});
        angularVel = new List<float>();
        angularVel.Add(0);
    }

    public void setMaxPoints(int points)
    {
        maxPoints = points;
    }
    public int getMaxPoints()
    {
        return maxPoints;
    }

    public List<Vector3[]> getTrajectory()
    {
        return trajectory;
    }


    public void update()
    {
        updateTrajectory(calcMotion(entity.transform.position));
        updateAVel(entity.transform.forward);
    }

    protected void updateTrajectory(Vector3[] next)
    {
        while (trajectory.Count >= maxPoints)
        {
            trajectory.RemoveAt(0);
        }
        trajectory.Add(next);
    }

    protected Vector3[] calcMotion(Vector3 pos)
    {
        Vector3[] cur = curMotion();
        Vector3 vel = (pos - cur[0]) / Time.fixedDeltaTime;
        Vector3 acc = (vel - cur[1]) / Time.fixedDeltaTime;
        return new Vector3[] { pos, vel, acc };
    }

    public Vector3[] curMotion()
    {
        return trajectory.Last();
    }
    public (Vector3 forward, float AVel) curAMotion()
    {
        return (forward, angularVel.Last());
    }

    protected void updateAVel(Vector3 dir)
    {
        while (angularVel.Count >= maxPoints)
        {
            angularVel.RemoveAt(0);
        }
        angularVel.Add(Vector3.Angle(dir, forward));
    }
    public List<float> getAVel()
    {
        return angularVel;
    }

    


    //Rotate around transform.position, axis Vector3, __ degrees
    //Calculate vector3 angular velocity with magnitude in radians
}
