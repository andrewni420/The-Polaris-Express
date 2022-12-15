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

    public void generateTrees(int distPerVertex, LevelData levelData)
    {
        int tileDepth = levelData.getTileSize().z;
        int tileWidth = levelData.getTileSize().x;
        int mapDepthInTiles = levelData.getSizeInTiles().z;
        int mapWidthInTiles = levelData.getSizeInTiles().x;

        generateLevelMap(distPerVertex, levelData);
        populateTrees(distPerVertex, levelData);
    }

    public void generateLevelMap(int distPerVertex, LevelData levelData)
    {
        int tileDepth = levelData.getTileSize().z;
        int tileWidth = levelData.getTileSize().x;
        int mapDepthInTiles = levelData.getSizeInTiles().z;
        int mapWidthInTiles = levelData.getSizeInTiles().x;

        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                levelData.setTreeMap(
                    noiseMapGeneration.GenerateNoiseMap(
                        tileDepth, tileWidth, distPerVertex, xTileIndex * tileWidth, zTileIndex * tileDepth, waves
                        ),
                    zTileIndex,
                    xTileIndex);
            }
        }
    }

    public void populateTrees(int distPerVertex, LevelData levelData)
    {

        (int z, int x) levelSize = levelData.getSizeInVertices();
        for (int zIndex = 0; zIndex < levelSize.z; zIndex++)
        {
            for (int xIndex = 0; xIndex < levelSize.x; xIndex++)
            {
    
                Vector3 vertex = levelData.getVertex(zIndex, xIndex);
                TerrainType terrainType = levelData.getBiome(zIndex, xIndex);
                if (terrainType.vegetation.Length > 0)
                {
                    
                    if (findMax(levelData, zIndex, xIndex, terrainType.neighborRadius))
                    {

                        Vector3 vegPos = new Vector3(xIndex * distPerVertex, vertex.y, zIndex * distPerVertex);
                        GameObject primaryVeg = Instantiate(terrainType.vegetation[0], vegPos, Quaternion.identity, transform) as GameObject;
                        
                        primaryVeg.transform.localScale = new Vector3(20, 20, 20);


                        //Spawn auxiliary vegetation close to primary vegetation - needs to account for mountain terrain
                        for (int i = 1; i < terrainType.vegetation.Length; i++)
                        {
                            (int x, int z) v = randomVertex(zIndex, xIndex, 2.1f, levelData);

                            Vector3 auxVertex = levelData.getVertex(v.z, v.x);
                            //int vPos = vertex.z * tileWidth + vertex.x;
                            Vector3 auxVegPos = new Vector3(v.x * distPerVertex, auxVertex.y, v.z * distPerVertex);
                            //Vector3 auxVegPos = levelData.getVertex(auxVertex.z, auxVertex.x);
                            GameObject auxVeg = Instantiate(terrainType.vegetation[i], auxVegPos, Quaternion.identity, transform) as GameObject;
                            auxVeg.transform.localScale = new Vector3(20, 20, 20);
                        }
                    }
                }
            }
        }

      
    }

    public (int x, int z) randomVertex(int zIndex, int xIndex, float radius, LevelData levelData)
    {
        (int z, int x) levelSize = levelData.getSizeInVertices();
        //Find start and end of area for random generation
        int zBegin = (int)Mathf.Max(0, zIndex - radius);
        int zEnd = (int)Mathf.Min(levelSize.z - 1, zIndex + radius);
        int xBegin = (int)Mathf.Max(0, xIndex - radius);
        int xEnd = (int)Mathf.Min(levelSize.x - 1, xIndex + radius);

        //Prevents infinite loops
        if (zBegin == zEnd || xBegin == xEnd) return (xBegin, zBegin); 
        int zPos = Random.Range(zBegin, zEnd + 1);
        int xPos = Random.Range(xBegin, xEnd + 1);

        //Try to avoid spawning auxiliary vegetation in same place as primary vegetation
        if (zPos != zIndex && xPos != xIndex) return (xPos, zPos);
        return randomVertex(zIndex, xIndex, radius, levelData);
    }

    public bool findMax(LevelData levelData, int zIndex, int xIndex, float neighborRadius)
    {
        float val = levelData.getTreeMap(zIndex, xIndex);
        (int z, int x) levelSize = levelData.getSizeInVertices();
        int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
        int neighborZEnd = (int)Mathf.Min(levelSize.z - 1, zIndex + neighborRadius);
        int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
        int neighborXEnd = (int)Mathf.Min(levelSize.x - 1, xIndex + neighborRadius);
        bool maxHit = false;

        for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
        {
            for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
            {
                float neighborValue = levelData.getTreeMap(neighborZ, neighborX);
                // saves the maximum tree noise value in the radius
                if (neighborValue > val) return false;
                if (neighborValue == val) maxHit = true;
            }
        }
        return maxHit;
    }
}
