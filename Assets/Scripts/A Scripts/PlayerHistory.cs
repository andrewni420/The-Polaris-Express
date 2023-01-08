using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "playerHistory", menuName = "Data Structs/playerHistory")]
public class PlayerHistory : ScriptableObject
{
    protected GameObject player;
    protected Trajectory trajectory;
    protected List<(bool attack, bool attackHit, bool attacked, bool attackedHit, bool block, bool jump)> combat;
    protected List<int> health;
    protected List<int> hunger;


    protected int maxPoints = 100;


    public void init(GameObject player)
    {
        this.player = player;
        trajectory = new Trajectory(player);
        combat = new List<(bool attack, bool attackHit, bool attacked, bool attackedHit, bool block, bool jump)>();
        health = new List<int>();
        hunger = new List<int>();
    }

    protected void enforceMaxPoints()
    {
        while (combat.Count > maxPoints)
        {
            combat.RemoveAt(0);
        }
        while (health.Count > maxPoints)
        {
            health.RemoveAt(0);
        }
        while (hunger.Count > maxPoints)
        {
            hunger.RemoveAt(0);
        }
    }

    public void updateAction((bool,bool,bool,bool,bool,bool) action)
    {
        addMotion();
        addCombat(action);
        enforceMaxPoints();
    }
    public void updateStats(int hunger, int health)
    {
        addHealth(health);
        addHunger(hunger);
        enforceMaxPoints();
    }

    public void addMotion()
    {
        trajectory.update();
    }

    public void addCombat((bool, bool, bool, bool, bool, bool) action)
    {
        combat.Add(action);
    }
    public void addHealth(int health)
    {
        this.health.Add(health);
    }
    public void addHunger(int hunger)
    {
        this.hunger.Add(hunger);
    }
    public Trajectory GetTrajectory()
    {
        return trajectory;
    }
    public List<(bool attack, bool attackHit, bool attacked, bool attackedHit, bool block, bool jump)> getCombat()
    {
        return combat;
    }
    public List<int> getHealth()
    {
        return health;
    }
    public List<int> getHunger()
    {
        return hunger;
    }

}
