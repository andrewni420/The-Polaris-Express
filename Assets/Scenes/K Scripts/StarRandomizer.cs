using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Resource for root Random Position Instantiation:
// https://www.youtube.com/watch?v=MVeI9y6k8MU
// video by UnityChat
public class StarRandomizer : MonoBehaviour
{
    public GameObject Star;

    public Terrain WorldTerrain;
    public LayerMask TerrainLayer;

    // Hard code these coordinates for each set of location coordinates
    public static float TerrainLeft, TerrainRight, TerrainTop, TerrainBottom, TerrainWidth, TerrainLength, TerrainHeight;

    
    // Call instantiate 3 times with diff set of things passed in calculated
    void Awake()
    {
        // pass in Terrain sections here?
        InstantiateRandomPosition("Prefabs/Star", 3, 0f);
        InstantiateRandomPosition("Prefabs/Star", 3, 0f);
        InstantiateRandomPosition("Prefabs/Star", 3, 0f);
    }

    // instatiates the game objects randomly
    // give it a specific range of coordinates for each section
    // hard wire the final destinations and store
    // their start pairs as a variable of the game property
    // in animation, access the locations and calculate the
    // curves for animation upon trigger from player running
    // into

    // what needs to get passed to this?
    public void calculateTerrainCoordinates() {
        TerrainLeft = WorldTerrain.transform.position.x;
        TerrainBottom = WorldTerrain.transform.position.z;
        TerrainWidth = WorldTerrain.terrainData.size.x;
        TerrainLength = WorldTerrain.terrainData.size.z;
        TerrainHeight = WorldTerrain.terrainData.size.y;
        TerrainRight = TerrainLeft + TerrainWidth;
        TerrainTop = TerrainBottom + TerrainLength;
    }

    public void InstantiateRandomPosition(string Resource, int Amount, float AddedHeight)
    {
        //define variables

        //loop through the amount of times we want to instantiare

        //generate random position within a range
        var i = 0;
        float terrainHeight = 0f;
        RaycastHit hit;
        float randomPositionX, randomPositionY, randomPositionZ;
        Vector3 randomPosition = Vector3.zero;

        do  {
            i++;
            randomPositionX = Random.Range(TerrainLeft, TerrainRight);
            randomPositionZ = Random.Range(TerrainBottom, TerrainTop);

            if (Physics.Raycast(new Vector3(randomPositionX, 9999f, randomPositionZ), Vector3.down, out hit, Mathf.Infinity, TerrainLayer)) {
                terrainHeight = hit.point.y;
            }

            randomPositionY = terrainHeight + AddedHeight;

            randomPosition = new Vector3(randomPositionX, randomPositionY, randomPositionZ);

            Instantiate(Resources.Load(Resource, typeof(GameObject)), randomPosition, Quaternion.identity);

        } while (i < Amount);
    }

    // public void pairCoordinatesWithFinal(){
        
    // }
}
