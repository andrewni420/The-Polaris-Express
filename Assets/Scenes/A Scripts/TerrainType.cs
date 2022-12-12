using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrain", menuName = "PCG/Terrain")]
public class TerrainType : GeneralTerrain
{
	public string TerrainName;
	public GameObject[] vegetation;
	public int numTiles;
    [HideInInspector]
	public List<int> triangles = new List<int>();
	public Material material;

    public override void initLevels(PDF pdf)
    {
    }

    public override void initTerrains()
    {
        triangles = new List<int>();
        terrains = new TerrainType[] { this };
    }
    public override GeneralTerrain chooseTerrain(Dictionary<string, float> x)
    {
        return this;
    }
    public override void resetNumTiles()
	{
		numTiles = 0;
	}
}