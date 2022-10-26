using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject player;
    private Trajectory playerTrajectory;
    private bool attack = false;
    // Start is called before the first frame update
    void Start()
    {
        playerTrajectory = new Trajectory(player,0F);
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

   


    public Vector3[] imputeTrajectory()
    {
        return playerTrajectory.getTrajectory();
    }

    public bool imputeAttack()
    {
        return attack;
    }
}
