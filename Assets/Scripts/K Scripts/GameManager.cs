using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Resource: https://youtu.be/VbZ9_C4-Qbo
// Brackeys
public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;

    public float restartDelay = 1f;

    public GameObject completeLevelUI;

    public  difficulty diff = difficulty.easy;


    public toughness enemyToughness = toughness.low;//modifier for health and damage
    public spawnRate spawnrate = spawnRate.low;//modifier for spawn cap and spawn chance
    public intelligence intLevel = intelligence.pos;//level of intelligence for interpreting player trajectory
    public gameArea area = gameArea.first;//modifiers for health, damage, spawn cap, and spawn chance
    public GameObject player;

    public (toughness t, spawnRate s, intelligence i, gameArea a) getSettings() { return (enemyToughness, spawnrate, intLevel, area);}
    
    void Awake()
    {
        SetDifficultyFromPlayerPrefs();
 
    }

    public void SetDifficultyFromPlayerPrefs()
    {
        string diffString = PlayerPrefs.GetString("difficulty", "easy");
        Debug.Log(diffString);
        diff = getDifficulty(diffString);
    }

    public difficulty getDifficulty(string d)
    {
        switch (d)
        {
            case "peaceful":
                return difficulty.peaceful;
            case "easy":
                return difficulty.easy;
                case "medium":
                return difficulty.medium;
            case "hard":
                return difficulty.hard;
            default:
                return difficulty.easy;
        }
    }
    public intelligence getIntelligence(string i)
    {
        switch (i)
        {
            case "pos":
                return intelligence.pos;
            case "vel":
                return intelligence.vel;
            case "none":
                return intelligence.none;
            default:
                return intelligence.pos;
        }
    }
    public spawnRate getSpawnRate(string s)
    {
        switch (s)
        {
            case "none":
                return spawnRate.none;
            case "low":
                return spawnRate.low;
            case "medium":
                return spawnRate.medium;
            case "high":
                return spawnRate.high;
            default:
                return spawnRate.low;
        }
    }
    public gameArea getGameArea(string g)
    {
        switch (g)
        {
            case "tutorial":
                return gameArea.tutorial;
            case "first":
                return gameArea.first;
            case "second":
                return gameArea.second;
            case "end":
                return gameArea.end;
            default:
                return gameArea.first;
        }
    }
    public toughness getToughness(string t)
    {
        switch (t)
        {
            case "low":
                return toughness.low;
            case "medium":
                return toughness.medium;
            case "high":
                return toughness.high;
            default:
                return toughness.low;
        }
    }
    (toughness t, intelligence i, spawnRate s) difficultyPrefab(difficulty d)
    {
        switch (d)
        {
            case difficulty.peaceful:
            case difficulty.easy:
                return (toughness.low, intelligence.pos, spawnRate.low);
            case difficulty.medium:
                return (toughness.medium, intelligence.pos, spawnRate.medium);
            case difficulty.hard:
                return (toughness.high, intelligence.vel, spawnRate.high);
            default:
                return (toughness.low, intelligence.none, spawnRate.none);
        }
    }
    public void initDifficulty(string gameDifficulty, string gameSpawnRate, string enemyIntLevel)
    {
        this.diff = getDifficulty(gameDifficulty);
        this.spawnrate = getSpawnRate(gameSpawnRate);
        this.intLevel = getIntelligence(enemyIntLevel);
        this.area = getGameArea("first");
    }
    public void setGameArea(string g)
    {
        this.area = getGameArea(g);
    }
    public void setGameArea(gameArea g)
    {
        this.area = g;
    }

    public void WinGame()
    {
        Debug.Log("WIN");
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Win Menu");
    }
    
    public void GameOver()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            Debug.Log("GAMEOVER");
            Invoke("Restart", restartDelay);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
