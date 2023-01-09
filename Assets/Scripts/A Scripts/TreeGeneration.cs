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
    public float distPerTree;

    public void generateTrees(int distPerVertex, LevelData levelData, float maxHeight)
    {
        distPerTree = Mathf.Max(distPerTree, 1);

        int tileDepth = levelData.getTileSize().z;
        int tileWidth = levelData.getTileSize().x;
        int mapDepthInTiles = levelData.getSizeInTiles().z;
        int mapWidthInTiles = levelData.getSizeInTiles().x;

        //generateLevelMap(distPerVertex, levelData);
        //populateTrees(distPerVertex, levelData);
        generateLevelMap2(levelData);
        populateTrees2(levelData, maxHeight);
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

    public void generateLevelMap2(LevelData levelData)
    {
        (float z, float x) size = levelData.getSizeInCoordinates();
        levelData.treeMap = noiseMapGeneration.GenerateNoiseMap((int)(size.z / distPerTree), (int)(size.x/distPerTree), 1, 0, 0, waves);
    }

    public void populateTrees2(LevelData levelData, float maxHeight)
    {
        (float z, float x) size = levelData.getSizeInCoordinates();
        for (int zIndex = 0; zIndex < (int)(size.z/distPerTree); zIndex++)
        {
            for (int xIndex = 0; xIndex < (int)(size.x / distPerTree); xIndex++)
            {

                Vector3 vertex = levelData.getVertexByCoord(zIndex*distPerTree, xIndex*distPerTree);
                TerrainType terrainType = levelData.getBiomeByCoord(zIndex*distPerTree, xIndex*distPerTree);
                if (terrainType.vegetation.Length > 0)
                {
                    if (findMaxCoord(levelData, zIndex, xIndex, terrainType.neighborRadius))
                    {
                        Vector3 vegPos;
                        if (levelData.project(new Vector3(xIndex*distPerTree, maxHeight+0.5f, zIndex*distPerTree), out vegPos))
                        {
                            GameObject primaryVeg = Instantiate(terrainType.vegetation[0], vegPos, Quaternion.identity, transform) as GameObject;
                            primaryVeg.transform.localScale = new Vector3(4, 4, 4);

                            //Spawn auxiliary vegetation close to primary vegetation - needs to account for mountain terrain
                            for (int i = 1; i < terrainType.vegetation.Length; i++)
                            {
                                (float x, float z) v = randomCoord(zIndex*distPerTree, xIndex*distPerTree, 2.1f, levelData);

                                Vector3 auxVertex = levelData.getVertexByCoord(v.z, v.x);
                                //int vPos = vertex.z * tileWidth + vertex.x;
                                Vector3 auxVegPos;
                                if (levelData.project(new Vector3(v.x, auxVertex.y, v.z), out auxVegPos))
                                {
                                    //Vector3 auxVegPos = levelData.getVertex(auxVertex.z, auxVertex.x);
                                    GameObject auxVeg = Instantiate(terrainType.vegetation[i], auxVegPos, Quaternion.identity, transform) as GameObject;
                                    auxVeg.transform.localScale = new Vector3(4, 4, 4);
                                }
                                
                            }
                        }
                        

                        


                       
                        
                    }
                }
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
                        
                        primaryVeg.transform.localScale = new Vector3(4, 4, 4);


                        //Spawn auxiliary vegetation close to primary vegetation - needs to account for mountain terrain
                        for (int i = 1; i < terrainType.vegetation.Length; i++)
                        {
                            (int x, int z) v = randomVertex(zIndex, xIndex, 2.1f, levelData);

                            Vector3 auxVertex = levelData.getVertex(v.z, v.x);
                            //int vPos = vertex.z * tileWidth + vertex.x;
                            Vector3 auxVegPos = new Vector3(v.x * distPerVertex, auxVertex.y, v.z * distPerVertex);
                            //Vector3 auxVegPos = levelData.getVertex(auxVertex.z, auxVertex.x);
                            GameObject auxVeg = Instantiate(terrainType.vegetation[i], auxVegPos, Quaternion.identity, transform) as GameObject;
                            auxVeg.transform.localScale = new Vector3(4, 4, 4);
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

    public (float x, float z) randomCoord(float zCoord, float xCoord, float radius, LevelData levelData)
    {
        radius = Mathf.Max(0.75f, radius);
        (float z, float x) size = levelData.getSizeInCoordinates();
        float zBegin = (int)Mathf.Max(0, zCoord - radius);
        float zEnd = (int)Mathf.Min(size.z, zCoord + radius);
        float xBegin = (int)Mathf.Max(0, xCoord - radius);
        float xEnd = (int)Mathf.Min(size.x, xCoord + radius);

        float xPos = Random.Range(xBegin, xEnd);
        float zPos = Random.Range(zBegin, zEnd);
        if (Vector2.Distance(new Vector2(xPos, zPos), new Vector2(xCoord, zCoord)) < 0.5) return randomCoord(zCoord, xCoord, radius, levelData);

        return (xPos, zPos);
    }

    public bool findMax(LevelData levelData, int zIndex, int xIndex, float neighborRadius)
    {
        float val = levelData.getTreeMap(zIndex, xIndex);
        //(int z, int x) levelSize = levelData.getSizeInVertices();
        int neighborZBegin = (int)Mathf.Max(0, zIndex - neighborRadius);
        int neighborZEnd = (int)Mathf.Min(levelData.treeMap.GetLength(0)-1, zIndex + neighborRadius);
        int neighborXBegin = (int)Mathf.Max(0, xIndex - neighborRadius);
        int neighborXEnd = (int)Mathf.Min(levelData.treeMap.GetLength(1) - 1, xIndex + neighborRadius);
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

    public bool findMaxCoord(LevelData levelData, int zCoord, int xCoord, float neighborRadius)
    {
        
        float val = levelData.treeMap[zCoord, xCoord];
        (float z, float x) size = levelData.getSizeInCoordinates();
        size = (size.z / distPerTree, size.x / distPerTree);
       
        int neighborZBegin = (int)Mathf.Max(0, zCoord - neighborRadius);
        int neighborZEnd = (int)Mathf.Min(size.z - 1, zCoord + neighborRadius);
        int neighborXBegin = (int)Mathf.Max(0, xCoord - neighborRadius);
        int neighborXEnd = (int)Mathf.Min(size.x - 1, xCoord + neighborRadius);

        bool maxHit = false;


        for (int neighborZ = neighborZBegin; neighborZ <= neighborZEnd; neighborZ++)
        {
            for (int neighborX = neighborXBegin; neighborX <= neighborXEnd; neighborX++)
            {
                float neighborValue = levelData.treeMap[neighborZ, neighborX];
                // saves the maximum tree noise value in the radius
                if (neighborValue > val) return false;
                if (neighborValue == val) maxHit = true;
            }
        }
        return maxHit;
    }
}
