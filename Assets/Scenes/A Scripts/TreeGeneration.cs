using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Adapted from Game Dev Academy https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
public class TreeGeneration : MonoBehaviour
{
    [SerializeField]
    private NoiseMapGeneration noiseMapGeneration;
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private float neighborRadius;

    public void GenerateTrees(int levelDepth, int levelWidth, float distanceBetweenVertices, TileData tileData)
    {
        // generate a tree noise map using Perlin Noise
        float[,] treeMap = this.noiseMapGeneration.GenerateNoiseMap(levelDepth, levelWidth, 0, 0, waves);
        float levelSizeX = levelWidth * distanceBetweenVertices;
        float levelSizeZ = levelDepth * distanceBetweenVertices;
        for (int zIndex = 0; zIndex < levelDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < levelWidth; xIndex++)
            {
                int tileWidth = tileData.heightMap.GetLength(1);
                // calculate the mesh vertex index
                Vector3[] meshVertices = tileData.mesh.vertices;
                int vertexIndex = zIndex * tileWidth + xIndex;
                // get the terrain type of this coordinate
                TerrainType terrainType = tileData.biomes[zIndex, xIndex];

                //First vegetation is primary. Rest are auxiliary and spawned around primary vegetation
                if (terrainType.vegetation.Length>0)
                {
                    //Spawn primary vegetation at local maximum of treeMap
                    if (findMax(treeMap, zIndex, xIndex, neighborRadius))
                    {
                        Vector3 vegPos = new Vector3(xIndex * distanceBetweenVertices, meshVertices[vertexIndex].y, zIndex * distanceBetweenVertices);
                        GameObject primaryVeg = Instantiate(terrainType.vegetation[0], vegPos, Quaternion.identity, transform) as GameObject;
                        primaryVeg.transform.localScale = new Vector3(1, 1, 1);

                        //Spawn auxiliary vegetation close to primary vegetation
                        for (int i = 1; i < terrainType.vegetation.Length; i++)
                        {
                            (int x, int z) vertex = randomVertex(zIndex, xIndex, neighborRadius/3, levelDepth, levelWidth);
                            int vPos = vertex.z * tileWidth + vertex.x;
                            Vector3 auxVegPos = new Vector3(vertex.x * distanceBetweenVertices, meshVertices[vPos].y, vertex.z * distanceBetweenVertices);
                            GameObject auxVeg = Instantiate(terrainType.vegetation[i], auxVegPos, Quaternion.identity, transform) as GameObject;
                            auxVeg.transform.localScale = new Vector3(10, 10, 10);
                        }
                    }
                }
            }
        }
    }

    public (int x, int z) randomVertex(int zIndex, int xIndex, float radius, int levelDepth, int levelWidth)
    {
        //Find start and end of area for random generation
        int zBegin = (int)Mathf.Max(0, zIndex - radius);
        int zEnd = (int)Mathf.Min(levelDepth - 1, zIndex + radius);
        int xBegin = (int)Mathf.Max(0, xIndex - radius);
        int xEnd = (int)Mathf.Min(levelWidth - 1, xIndex + radius);

        //Prevents infinite loops
        if (zBegin == zEnd || xBegin == xEnd) return (xBegin, zBegin); 
        int zPos = Random.Range(zBegin, zEnd + 1);
        int xPos = Random.Range(xBegin, xEnd + 1);

        //Try to avoid spawning auxiliary vegetation in same place as primary vegetation
        if (zPos != zIndex && xPos != xIndex) return (xPos, zPos);
        return randomVertex(zIndex, xIndex, neighborRadius, levelDepth, levelWidth);
    }

    public bool findMax(float[,] treeMap, int zIndex, int xIndex, float neighborRadius)
    {
        float treeValue = treeMap[zIndex, xIndex];
        int levelDepth = treeMap.GetLength(0);
        int levelWidth = treeMap.GetLength(1);
        // compares the current tree noise value to the neighbor ones
        int neighborZBegin = (int)Mathf.Max(0, zIndex - this.neighborRadius);
        int neighborZEnd = (int)Mathf.Min(levelDepth - 1, zIndex + this.neighborRadius);
        int neighborXBegin = (int)Mathf.Max(0, xIndex - this.neighborRadius);
        int neighborXEnd = (int)Mathf.Min(levelWidth - 1, xIndex + this.neighborRadius);
        bool maxHit = false;
        for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
        {
            for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
            {
                float neighborValue = treeMap[neighborZ, neighborX];
                // saves the maximum tree noise value in the radius
                if (neighborValue > treeValue) return false;
                if (neighborValue == treeValue) maxHit = true;

            }
        }
        return maxHit;
    }
}
