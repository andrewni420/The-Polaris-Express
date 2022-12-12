using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainSet", menuName = "PCG/TerrainSet")]
public class TerrainSet : GeneralTerrain
{
	public GeneralTerrain[] _terrains;

	public float[] levels;

	public bool usePreset = false;

	public string modifier;

	public override void initLevels(PDF pdf)
	{
		foreach (GeneralTerrain s in _terrains) s.initLevels(pdf);
		if (usePreset) return;
		int numLevels = _terrains.Length;
		float[] setLevels = new float[numLevels];
		for (int i = 1; i <= numLevels; i++) setLevels[i - 1] = (float)i / numLevels;
		levels = pdf.levelCurves(setLevels, 100);
	}
	public override void initTerrains()
	{
		HashSet<TerrainType> h = new HashSet<TerrainType>();
		foreach (GeneralTerrain s in _terrains)
		{
			s.initTerrains();
			h.UnionWith(s.terrains);
		}
		terrains = new TerrainType[h.Count];
		h.CopyTo(terrains);
	}

	public override GeneralTerrain chooseTerrain(Dictionary<string, float> x)
	{
		if (!x.ContainsKey(modifier)) return _terrains[0].chooseTerrain(x);
		for (int i = 0; i < levels.Length; i++)
		{
			if (x[modifier] < levels[i])
			{
				return _terrains[i].chooseTerrain(x);
			}
		}
		return _terrains[0].chooseTerrain(x);
	}

	public override void resetNumTiles()
	{
		foreach (GeneralTerrain s in _terrains) s.resetNumTiles();
	}

}
