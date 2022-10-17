using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject player;
    private Vector3[] playerTrajectory;
    private bool attack;
    // Start is called before the first frame update
    void Start()
    {
        attack = false;
        playerTrajectory = new Vector3[] { player.transform.position, new Vector3(), new Vector3() };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) attack = true;
        else attack = false;
    }

    void FixedUpdate()
    {
        playerTrajectory = calcTrajectory(playerTrajectory);
    }

    Vector3[] calcTrajectory(Vector3[] curTrajectory)
    {
        Vector3 curPos = player.transform.position;
        Vector3 curVel = (curPos - curTrajectory[0]) / Time.fixedDeltaTime;
        Vector3 curAcc = (curVel - curTrajectory[1]) / Time.fixedDeltaTime;
        return new Vector3[] { curPos, curVel, curAcc };
    }


    public Vector3[] imputeTrajectory()
    {
        return playerTrajectory;
    }

    public bool imputeAttack()
    {
        return attack;
    }
}
