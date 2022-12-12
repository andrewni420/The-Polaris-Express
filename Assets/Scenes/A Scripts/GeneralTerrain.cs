using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class GeneralTerrain : ScriptableObject
{
	[HideInInspector]
	public TerrainType[] terrains;

	public abstract void initLevels(PDF pdf);
	public abstract void initTerrains();

	public abstract GeneralTerrain chooseTerrain(Dictionary<string,float> x);

	public abstract void resetNumTiles();

	public void initRenderer(MeshRenderer r)
	{
		Material[] m = new Material[terrains.Length];
		int i = 0;
		foreach (TerrainType t in terrains)
		{
			m[i] = t.material;
			i++;
		}
		r.materials = m;
	}

	public void initMesh(Mesh m)
	{
		m.subMeshCount = terrains.Length;
		int i = 0;
		foreach (TerrainType t in terrains)
		{
			Debug.Log((t.TerrainName, t.triangles.Count));
			m.SetTriangles(t.triangles, i);
			i++;
		}
	}
}
