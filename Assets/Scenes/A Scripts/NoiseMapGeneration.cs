using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//Parts adapted from Game Dev Academy https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
public class NoiseMapGeneration : MonoBehaviour {
	public delegate float function(float noise, float x, float z);
	public function heightFunction = (float noise, float x, float y) => noise;//must be between 0 and 1

	public void setHeightFunction(function f)
    {
		heightFunction = f;
    }
	public void removeHeightFunction()
    {
		heightFunction = (float noise, float x, float y) => noise;

	}

	private void updateArr(float noise, int[] arr)
    {
		noise = Mathf.Min(Mathf.Max(noise,0.000000001f),0.999999999f);
		arr[(int)Mathf.Floor(noise*arr.Length)]++;
    }

	public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, int distPerVertex, float offsetX, float offsetZ, Wave[] waves)
	{

		// create an empty noise map with the mapDepth and mapWidth coordinates
		float[,] noiseMap = new float[mapDepth, mapWidth];

		for (int zIndex = 0; zIndex < mapDepth; zIndex++)
		{
			for (int xIndex = 0; xIndex < mapWidth; xIndex++)
			{
				// calculate sample indices based on the coordinates, the scale and the offset
				float sampleX = xIndex*distPerVertex + offsetX;
				float sampleZ = zIndex*distPerVertex + offsetZ;

				float noise = 0f;
				float normalization = 0f;
				foreach (Wave wave in waves)
				{
					// generate noise value using PerlinNoise for a given Wave
					noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
					normalization += wave.amplitude;
				}
				// normalize the noise value so that it is within 0 and 1
				noise /= normalization;


				noise = heightFunction(noise, zIndex/(float)(mapDepth), xIndex/(float)mapWidth);
				//noise = Math.Min(Math.Max(noise, 0), 1);

				noiseMap[zIndex, xIndex] = noise;
			}
		}
		return noiseMap;
	}


}

[System.Serializable]
public class Wave {
	public float seed;
	public float frequency;
	public float amplitude;
}
