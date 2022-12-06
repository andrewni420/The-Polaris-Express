using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Compute level curves of Perlin noise
//Use levelCurves array to set the moisture levels of biomes in PCG generation to ensure equal areas for each biome
public class ComputeLevelCurves : MonoBehaviour
{
	public float[] levels;
	public float[] levelCurves;
	public PDF PerlinPDF;

	private int numSamples = 100;

    // Start is called before the first frame update
    void Start()
    {
		GenerateNoiseMap();
		//PerlinPDF.normalizePDF();
		levelCurves = PerlinPDF.levelCurves(levels,100);
		foreach (float i in levelCurves) Debug.Log(i);
    }


	public void GenerateNoiseMap()
	{
		float zMin = Random.Range(-10000f,10000f);
		float xMin = Random.Range(-10000f,10000f);
		float scale = 0f;
		while (scale%1==0) scale = Mathf.Exp(Random.Range(-5f,3f));

		float[] pdf = new float[100];
		for (int zIndex = 0; zIndex < numSamples; zIndex++)
		{
			for (int xIndex = 0; xIndex < numSamples; xIndex++)
			{
				float sampleX = xMin+xIndex * scale;
				float sampleZ = zMin+zIndex * scale;

				float noise = getNoise(sampleX, sampleZ);
				pdf[(int)Mathf.Floor(noise * pdf.Length)]++;
			}
		}

		PerlinPDF.normalize(pdf);
		PerlinPDF.updatePDF(pdf);
	}

	public float getNoise(float sampleX, float sampleZ)
    {
		float noise = Mathf.PerlinNoise(sampleX, sampleZ);
		noise = Mathf.Min(Mathf.Max(noise, 0), 0.999f);
		return noise;
	}

}
