using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum intelligence
{
    pos, vel, none
}
public enum difficulty
{
    //Peaceful = no aggressive enemies
    //Easy = few spawns, low toughness
    //Medium = medium spawns, medium toughness
    //Hard = lots of spawns, very tough
    peaceful, easy, medium, hard
}
public enum spawnRate
{
    none, low, medium, high
}
public enum gameArea
{
    tutorial, first, second, end
}
public enum toughness
{
    low, medium, high
}

public class Spawner : MonoBehaviour
{
    

    public GameObject aggressivePrefab;//ranged (bow or item), tack shooter / spinny boi, ... There needs to be a distribution for these too
    //Have a special smart enemy that uses machine learning
    //Javelin = ranged melee hybrid. One shot then charges in
    //Assassin aims to flank the player
    //Higher int level means strategies like baiting attack?
    //Scripted enemy spawns?
        //e.g.Patrols
    //PCG to randomly allocate 
    public GameObject passivePrefab;
    public GameObject scaredPrefab;
    public GameObject healthPrefab;
    public GameObject player;
    public List<GameObject> Enemies;
    public GameManager gameManager;

    /// <summary>

    /// </summary>
    public AI playerPrediction;//Predicts player movement
    public FPCam cam;
    public GameObject HealthCanvas;

    private int spawnCap = 5;
    private float spawnProb = 0.01f;

    // Initialization
    void Start()
    {
        Enemies = new List<GameObject>();
    }
    
    

    //Convert strings to enums


    //Get prefab for each difficulty
    

    //Get stats of enemies
    (float health, float damage) getModifier(toughness t)
    {
        switch (t)
        {
            case toughness.low:
                return (0.75f,0.75f);
            case toughness.medium:
                return (1f,1f);
            case toughness.high:
                return (1.5f,1.5f);
            default:
                return (1f, 1f);
        }
    }
    (float health, float damage) getModifier(gameArea g)
    {
        //Tutorial = super low stakes, super fast
        //area 1 = normal
        //area 2 = higher health. Explore combat options without fights ending too fast
        //area 3 = hardest
        //Also want to make older weapons obsolete
        switch (g)
        {
            case gameArea.tutorial:
                return (0.5f, 0.25f);
            case gameArea.first:
                return (1f, 1f);
            case gameArea.second:
                return (2f, 1f);
            case gameArea.end:
                return (2.5f, 2.5f);
            default:
                return (1f, 1f);
        }
    }
    (int health, int damage, int knockback) getStats()
    {
        //Gaussian distribution of health and damage around mean.
        //Enemies mostly within 10% deviation from mean
        //Ensure nonnegative health and damage
        var settings = gameManager.getSettings();
        float health = 100*getModifier(settings.t).health*getModifier(settings.a).health;
        float healthStd = health / 30;
        health = Math.Max(0,randNormal(health, healthStd));
        float damage = 10 * getModifier(settings.t).damage * getModifier(settings.a).damage;
        float damageStd = damage / 30;
        damage = Math.Max(0,randNormal(damage, damageStd));
        return ((int)health, (int)damage,5);
    }

    //To spawn, you have a spawnCap, and a spawnProbability. multiplier modifies both
    //Every update, check if number of enemies is under cap. Then there is a probability for a spawn to happen. Then randomize spawned type by enemy distribution
    (float cap, float chance) spawnMultiplier(spawnRate rate)
    {
        switch (rate)
        {
            case spawnRate.none:
                return (0f, 0f);
            case spawnRate.low:
                return (1f, 1f);
            case spawnRate.medium:
                return (1.5f, 1f);
            case spawnRate.high:
                return (2f, 2f);
            default:
                return (1f, 1f);
        }
    }
    (float passive, float aggressive, float scared) enemyDistribution(gameArea g)
    {
        switch (g)
        {
            case gameArea.tutorial:
                return (1f, 1f, 1f);
            case gameArea.first:
                return (2f, 1f, 2f);
            case gameArea.second:
                return (1f, 2f, 2f);
            case gameArea.end:
                return (1f, 4f, 2f);
            default:
                return (1f, 1f, 1f);
        }

    }
    (float cap, float chance) spawnMultiplier(gameArea g)
    {
        //Only 3 enemies allowed in tutorial
        //Area 1 = normal
        //Area 2 = cap raised. Be careful about too many enemies accumlating
        //Area 3 = cap and rate raised.
        switch (g)
        {
            case gameArea.tutorial:
                return (3f / spawnCap, 0f);
            case gameArea.first:
                return (1f, 1f);
            case gameArea.second:
                return (2f, 1f);
            case gameArea.end:
                return (2f, 2f);
            default:
                return (1f, 1f);
        }
    }
    (int cap, float chance) spawnParams()
    {
        var settings = gameManager.getSettings();
        float cap = spawnCap * spawnMultiplier(settings.s).cap * spawnMultiplier(settings.a).cap;
        float chance = spawnProb * spawnMultiplier(settings.s).chance * spawnMultiplier(settings.a).chance;
        return ((int)cap, chance);
    }

    //Gaussian distribution
    float randNormal(float mean, float std)
    {
        float u1 = UnityEngine.Random.Range(0f, 1f);
        float u2 = UnityEngine.Random.Range(0f, 1f);
        while (u1 == 0f) u1 = UnityEngine.Random.Range(0f, 1f);
        while (u2 == 0f) u2 = UnityEngine.Random.Range(0f, 1f);
        float zScore = (float)(Math.Sqrt(-2f * Math.Log(u1)) * Math.Sin(2f * Math.PI * u2));
        return mean + std * zScore;
    }

    //Random point on a circle
    Vector3 randomCircle(Vector3 center, float radius)
    {
        Vector2 delta = UnityEngine.Random.insideUnitCircle * radius;
        return center + new Vector3(delta.x, 0f, delta.y);
    }

    //Attempt to find nearest point on navmesh
    bool projectNavMesh(Vector3 point, out Vector3 projected)
    {
        projected = point;
        UnityEngine.AI.NavMeshHit hit;
        float maxDistance = 2f;
        //generate points exponentially farther away -> linear time
        if (!UnityEngine.AI.NavMesh.SamplePosition(point, out hit, maxDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            return false;
        }

        projected = hit.position;
        return true;
    }

    //Spawn a random enemy around pos using a gaussian distribution with standard deviation std
    bool spawnRandom(Vector3 pos, float std, out GameObject enemy)
    {
        enemy = null;

        Vector3 enemyPos = new Vector3();
        //try up to 10 times to generate an enemy
        for (int i = 0; i < 10; i++)
        {
            float random = randNormal(0f, std);
            Vector2 posDelta = UnityEngine.Random.insideUnitCircle * random;
            Vector3 position = pos + new Vector3(posDelta.x, 0f, posDelta.y);
            if (projectNavMesh(position, out enemyPos)) break;
        }
        var settings = gameManager.getSettings();
        (float passive, float aggressive, float scared) distribution = enemyDistribution(settings.a);
        if (gameManager.diff == difficulty.peaceful) distribution = (distribution.passive, 0f, distribution.scared);

        float enemyType = UnityEngine.Random.Range(0f, distribution.aggressive + distribution.passive + distribution.scared);

        if (enemyType < distribution.aggressive) enemy = Instantiate(aggressivePrefab, enemyPos, Quaternion.identity);
        else if (enemyType < distribution.aggressive + distribution.scared) enemy = Instantiate(scaredPrefab, enemyPos, Quaternion.identity);
        else enemy = Instantiate(passivePrefab, enemyPos, Quaternion.identity);
        return true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Clean up enemies list
        Enemies.RemoveAll(item => item==null);
        //Retrieve current spawn parameters
        (int cap, float chance) spawnparams = spawnParams();

        //If enemies under cap and probability met
        if (Enemies.Count < spawnparams.cap && UnityEngine.Random.Range(0f, 1f)<spawnparams.chance)
        {
            //Spawn in a circle of radius 15 centered at player position using a gaussian distribution with std 5.
            Vector3 center = player.transform.position;
            float std = 5f;
            float radius = 15f;
            Vector3 pos = randomCircle(center,radius);

            var settings = gameManager.getSettings();
            GameObject enemy;
            if (spawnRandom(pos, std, out enemy))
            {
                //Initialize enemy
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                (int health, int damage,int knockback) enemyStats = getStats();
                enemyScript.setStats(enemyStats.health, enemyStats.health, enemyStats.damage, enemyStats.knockback);
                enemyScript.setInt(settings.i);
                enemyScript.playerPrediction = this.playerPrediction;

                //Add to list
                Enemies.Add(enemy);

                //Add health bar to enemy
                GameObject healthBar = Instantiate(healthPrefab);
                healthBar.GetComponent<EnemyHealthView>().enemyObject = enemy;
                healthBar.GetComponent<EnemyHealthView>().cam = cam;
                healthBar.transform.SetParent(HealthCanvas.transform,false);
            }
        }

    }

    
    


    
}
