using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement//EnemyAction
{
    //MoveTowards
    //Intercept
    //MoveAway
    //Flank
    //PassiveMove
    //  updateDestination
    //  gotodestination
    //  updateRotate
    //  rotate

    //Stand

    //Enemy archetypes
    //Aggressive
    //Passive
    //Bait
    //Ranged
    //  (reload needed, try to reload), try attack, flank side
    //  try reload
    //      (player distracted, reload), (player distance, reload), escape
    //      escape
    //          (tank nearby, run to tank), (player distance, move away), attack
    //  try attack
    //      (player distracted, player distance, attack)
    //Tank
    //  (Need for distraction, rush), (Need for shield, shield), flank side
    //      shield
    //          (Find highest priority = health/fragility/player direction, find intermediate, move to position), (player distance, attack)
    //Glass cannon
    //  (Analyze safety, flank side) flank back
    //

    //Combat
    //Attack
    //Throw item
    //shield ranged
    //pressure = intercept
    //Taunt = bait a missed swing

    //Swarm behavior (low FOV, low strength, low health, low speed, but combined health bar, herd behavior and? regenerate members) Bait player into ambush with a weak guy?
    //  Combined strength vs player strength
    //  ">>" = rush, ">" = (check health, rush), flank "==" = (check health, flank side), flank back, "<<" = retreat
    //  Player seen
    //      (check FOV, alert others)
    //  Stay together
    //  Wander
    //  Regen members

    //rush
    //  move toward target
    //      (isdead, attack), clear target
    //flank side - success = flanked, running = neither
    //  (player too close, (pick side, gotoside)), rush
    //flank back
    //  (need for attack = flanking success or player too close, rush)
    //  (check relative position, move to side [farther away than flank side]), move to back
    //retreat
    //  get reverse direction
    //  move to position


    protected Queue<Vector3> predTraj = new Queue<Vector3>();
    protected Queue<float> predAVel = new Queue<float>();
    private Enemy enemy;

    public EnemyMovement(GameObject enemy)
    {
        this.enemy = enemy.GetComponent<Enemy>();
    }

    public (Vector3 dir, Vector3 rot) getNextMove(enemyState s, Vector3[] playerTrajectory)
    {
        switch (s)
        {
            case enemyState.passive:
                return passiveMove(playerTrajectory);
            case enemyState.aggressive:
                return aggressiveMove(playerTrajectory);
            case enemyState.scared:
                return scaredMove(playerTrajectory);
            default:
                return passiveMove(playerTrajectory);
        }
    }

    private (Vector3 dir, Vector3 rot) aggressiveMove(Vector3[] playerTrajectory)
    {
        float speed = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed;
        Vector3 nextMove;
        switch (enemy.GetComponent<Enemy>().intLevel)
        {
            case intelligence.pos:
                nextMove = playerTrajectory[0];
                break;
            case intelligence.vel:
                nextMove = playerTrajectory[0] + playerTrajectory[1] * (enemy.transform.position - playerTrajectory[0]).magnitude / speed;
                break;
            default:
                nextMove = enemy.transform.position;
                break;
        }
        return (nextMove, new Vector3());
    }

    private (Vector3 dir, Vector3 rot) scaredMove(Vector3[] playerTrajectory)
    {
        Vector3 nextMove;
        switch (enemy.GetComponent<Enemy>().intLevel)
        {
            case intelligence.pos:
            case intelligence.vel:
                nextMove = enemy.transform.position + (enemy.transform.position - playerTrajectory[0]).normalized;
                break;
            default:
                nextMove = enemy.transform.position;
                break;
        }
        return (nextMove, new Vector3());
    }

    private (Vector3 dir, Vector3 rot) passiveMove(Vector3[] playerTrajectory)
    {
        if (predTraj.Count < 5) predictMovement();
        Vector3 nextTraj = popPredTraj();
        float nextAngle = popPredAVel();

        return (nextTraj, new Vector3(0, nextAngle, 0));
    }

    public Queue<Vector3> getPredTraj() { return predTraj; }
    public Vector3 popPredTraj() { return predTraj.Dequeue(); }
    public Queue<float> getPredAVel() { return predAVel; }
    public float popPredAVel() { return predAVel.Dequeue(); }

    public void addForwardTraj()
    {
        int moveLength = UnityEngine.Random.Range(60, 100);
        int moveDist = UnityEngine.Random.Range(10, 30);
        Vector3 forward = enemy.transform.position + enemy.transform.forward * moveDist;
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
        float rotSpeed = UnityEngine.Random.Range(0.8F, 1F) * enemy.maxRotSpeed * leftRight;
        for (int i = 0; i < rotLength; i++)
        {
            predAVel.Enqueue(rotSpeed);
            predTraj.Enqueue(enemy.transform.position);
        }
    }
    public void addWaitTraj()
    {
        int waitLength = UnityEngine.Random.Range(20, 40);
        for (int i = 0; i < waitLength; i++)
        {
            predTraj.Enqueue(enemy.transform.position);
            predAVel.Enqueue(0);
        }
    }
    public void predictMovement()
    {
        //Movement types: move forward / rotate / wait
        float moveType = UnityEngine.Random.Range(0F, 1F);

        if (moveType < 0.33) addForwardTraj();
        else if (moveType < 0.66) addRotationTraj();
        else addWaitTraj();
    }

}
