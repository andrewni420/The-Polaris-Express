using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour//Scriptable Object
{
    public GameObject player;
    private Trajectory playerTrajectory;
    private bool attack = false;
    //ML prediction. Given player and all agg enemies, predict next movement and action. 
    // Start is called before the first frame update
    void Start()
    {
        playerTrajectory = new Trajectory(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) attack = true;
        else attack = false;
    }

    void FixedUpdate()
    {
        playerTrajectory.update();
    }

   


    public Vector3[] imputeMotion()
    {
        return playerTrajectory.curMotion();
    }

    public bool imputeAttack()
    {
        return attack;
    }
}
