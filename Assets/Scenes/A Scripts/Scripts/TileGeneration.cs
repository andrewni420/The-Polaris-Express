using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

//Parts adapted from Game Dev Academy https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
//Parts adapted from Catlike Coding https://catlikecoding.com/unity/tutorials/procedural-grid/
public class TileGeneration : MonoBehaviour {

	[SerializeField]
	NoiseMapGeneration noiseMapGeneration;

	public NavMeshSurface surface;

	[SerializeField]
	private MeshRenderer meshRenderer;
	[SerializeField]
	private MeshFilter meshFilter;
	[SerializeField]
	private MeshCollider meshCollider;
	[SerializeField]
	private TreeGeneration treeGenerator;

	private Mesh mesh;
	private int xSize = 200;
	private int ySize = 200;
	float[,] heightMap;
	float[,] moistureMap;
	TerrainType[,] biomes;

	//Biome moisture levels determined using ComputeLevelCurves.cs
	[SerializeField]
	private TerrainType[] terrainTypes;

	[SerializeField]
	private float heightMultiplier;

	[SerializeField]
	private float groundMountainRatio;


	//Use perlin waves to generate fractal noise
	[SerializeField]
	private Wave[] heightWaves;
	[SerializeField]
	private Wave[] mountainWaves;
	[SerializeField]
	private Wave[] moistureWaves;

	void Start() {
		foreach (TerrainType t in terrainTypes) t.numTiles = 0;

		initializeWaves((int)System.DateTime.Now.Ticks);

		heightMap = new float[xSize+1, ySize+1];
		biomes = new TerrainType[xSize + 1, ySize + 1];
		noiseMapGeneration.setHeightFunction(heightFunction);
		GenerateGrid();
		generateMaps();
		ensureCompleteMaps();
		setTriangles();
		GenerateTile ();
		GenerateTrees();
	}

	void GenerateTrees()
    {
		TileData data = new TileData(heightMap, moistureMap, biomes, mesh);

		Vector3 tileSize = meshRenderer.bounds.size;
		int tileWidth = (int)tileSize.x;
		int tileDepth = (int)tileSize.z;
		// calculate the number of vertices of the tile in each axis using its mesh
		int tileDepthInVertices = (int) Mathf.Sqrt(mesh.vertices.Length);
		int tileWidthInVertices = tileDepthInVertices;

		float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;

		treeGenerator.GenerateTrees(tileDepthInVertices, tileWidthInVertices, distanceBetweenVertices, data);
    }

	void initializeWaves(int seed)
    {
		int length = heightWaves.Length + mountainWaves.Length + mountainWaves.Length;
		Random.InitState(seed);
		int curSeed = 0;
		int[] seeds = new int[length];
		for (int i = 0; i < seeds.Length; i++)
        {
			//guarantees no overlap of waves with frequency up to 10
			seeds[i] = Random.Range(curSeed + 10*xSize, curSeed + 100*xSize);
			curSeed = seeds[i];
        }

		int j = 0;
		foreach (Wave w in heightWaves) w.seed = seeds[j++];
		foreach (Wave w in mountainWaves) w.seed = seeds[j++];
		foreach (Wave w in moistureWaves) w.seed = seeds[j++];

	}

	float heightFunction(float noise, float x, float y)
    {
		float gmr = groundMountainRatio;
		float xdist = Mathf.Abs(x - 0.5f);
		float ydist = Mathf.Abs(y - 0.5f);

		float d = Mathf.Sqrt(xdist * xdist + ydist * ydist);
		if (d < 0.05)
		{
			//d goes from 0 at the edge of the hill to 1 at the center
			d = (0.05f - d) / 0.05f;
			d = Mathf.Min(d * 9 / 6, d/4f+0.75f);

			//d = 2 * Mathf.Min(d, 0.75f);

			//Sin produces a nice rounded curve appropriate for a small hill. Cap it at 1/3 since it's a small hill
			//float r = Mathf.Sin(d * Mathf.PI / 2)/4f;
			//linearly increase noise amplitude
			//return Mathf.Pow(7, d) / 10 - 0.1f + (gmr + (0.4f - gmr) * d) * noise;//
			return (gmr+0.2f*d)*noise+d/4;
		}

		if (xdist<0.4 && ydist <0.4) return gmr*noise;

		//Use L-infinity norm to make equidistant surfaces in the shape of squares
		//Linear function to go from 0 at 0.40 to 1 at 0.47 and back down.
		//Positioning max at 0.47 prevents an awkward patch of 0 height terrain at edge of world;
		float s = (0.07f-Mathf.Abs(0.47f-Mathf.Max(xdist, ydist)))/0.07f;

		//Exponential function to slowly transition from 0 to 0.6 prevents "ground level" biomes from going too high on the mountains
		//Linearly increase amplitude of noise to smoothly transition into mountain peaks
		return Mathf.Pow(7, s) / 10 - 0.1f + (gmr + (0.4f-gmr) * s) * noise;
    }

	private void GenerateGrid()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0, y = 0; y <= ySize; y++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(5*x, 0, 5*y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;

	}

	void setTriangles()
    {
		mesh.subMeshCount = 6;
		List<int> grassTriangles = new List<int>();
		List<int> wheatTriangles = new List<int>();
		List<int> forestTriangles = new List<int>();
		List<int> rockTriangles = new List<int>();

		List<int> mountainTriangles = new List<int>();
		List<int> snowTriangles = new List<int>();

		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				List<int> list;
				switch (biomes[y,x].name)
                {
					case "grass":
						list = grassTriangles;
						break;
					case "wheat":
						list = wheatTriangles;
						break;
					case "forest":
						list = forestTriangles;
						break;
					case "rock":
						list = rockTriangles;
						break;
					case "mountain":
						list = mountainTriangles;
						break;
					case "snow":
						list = snowTriangles;
						break;
					default:
						list = grassTriangles;
						break;
                }
				list.Add(vi);
				list.Add(vi + xSize + 1);
				list.Add(vi + 1);
				list.Add(vi + 1);
				list.Add(vi + xSize + 1);
				list.Add(vi + xSize + 2);
			}
		}
		mesh.SetTriangles(grassTriangles, 0);
		mesh.SetTriangles(wheatTriangles, 1);
		mesh.SetTriangles(forestTriangles, 2);
		mesh.SetTriangles(rockTriangles, 3);
		mesh.SetTriangles(mountainTriangles, 4);
		mesh.SetTriangles(snowTriangles, 5);
	}

	void ensureCompleteMaps()
    {
		int count = 10;
		while(count>0 && incompleteBiomes())
        {
			generateMaps();
			Debug.Log(count);
		}
		
	}

	bool incompleteBiomes()
    {
		int numVertices = (xSize + 1) * (ySize + 1);
		int minVertices = numVertices / 5;
		foreach (TerrainType t in terrainTypes)
        {
			if (t.numTiles < minVertices && t.name!="mountian" && t.name!="snow") return false;
        }
		return true;
    }

	void generateMaps()
    {
		// calculate tile depth and width based on the mesh vertices
		Vector3[] meshVertices = this.meshFilter.mesh.vertices;
		int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
		int tileWidth = tileDepth;

		// calculate the offsets based on the tile position
		float offsetX = -this.gameObject.transform.position.x;
		float offsetZ = -this.gameObject.transform.position.z;

		// generate a heightMap using noise
		float[,] heightNoise = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ, heightWaves);
		float[,] mountainNoise = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ, mountainWaves);
		noiseMapGeneration.removeHeightFunction();
		moistureMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, offsetX, offsetZ, moistureWaves);

		//Combine height and mountain noise into heightmap
		for (int zIndex = 0; zIndex < tileDepth; zIndex++)
		{
			for (int xIndex = 0; xIndex < tileWidth; xIndex++)
			{
				heightMap[zIndex, xIndex] = combineMaps(heightNoise[zIndex, xIndex], mountainNoise[zIndex, xIndex], zIndex / (float)tileDepth, xIndex / (float)tileWidth);
				biomes[zIndex, xIndex] = ChooseTerrainType(heightMap[zIndex, xIndex], moistureMap[zIndex, xIndex]);
				biomes[zIndex, xIndex].numTiles++;
			}
		}
	}

	//Combines a value m1 from heightNoise and a value m2 from mountainNoise into a value for heightMap depending on the coordinates
	float combineMaps(float m1, float m2, float x, float y)
    {
		//L-infinity distance from center. Set Linf=0 when distance = 0.4, the transition from normal to mountain terrain
		float Linf = Mathf.Max(Mathf.Abs(x - 0.5f), Mathf.Abs(y - 0.5f))-0.4f;

		//L2 distance from center. Set L2=0 when distance=0.05, the transition from normal to hill terrain
		x -= 0.5f;
		y -= 0.5f;
		float L2 = Mathf.Sqrt(x * x + y * y)-0.05f;


		//Sigmoid to smoothly transition from hill generation to normal generation to mountainous generation
		float s1 = 1-sigmoid(100 * Linf);
		float s2 = sigmoid(200 * L2);


		//Describes transition from hill terrain to previous two terrains
		return (1-s1*s2)*m2+s1*s2*m1;
	}

	float sigmoid(float x) { return 1 / (1 + Mathf.Exp(-x)); }

	void GenerateTile() {

		// build a Texture2D from the height map
		foreach (Material material in this.meshRenderer.materials)
		{
			material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		}

		// update the tile mesh vertices according to the height map
		UpdateMeshVertices (heightMap);
		
	}

	TerrainType ChooseTerrainType(float height, float moisture) {
		// for each terrain type, check if the height is lower than the one for the terrain type
		foreach (TerrainType terrainType in terrainTypes) {
			// return the first terrain type whose height is higher than the generated one
			if (height <= terrainType.maxHeight && height >= terrainType.minHeight &&
				moisture <= terrainType.maxMoisture && moisture >= terrainType.minMoisture) {
				return terrainType;
			}
		}
		return terrainTypes [0];
	}

	private void UpdateMeshVertices(float[,] heightMap) {
		this.meshFilter.mesh = mesh;

		int tileDepth = heightMap.GetLength (0);
		int tileWidth = heightMap.GetLength (1);

		Vector3[] meshVertices = this.meshFilter.mesh.vertices;

		// iterate through all the heightMap coordinates, updating the vertex index
		int vertexIndex = 0;
		for (int zIndex = 0; zIndex < tileDepth; zIndex++) {
			for (int xIndex = 0; xIndex < tileWidth; xIndex++) {
				float height = heightMap [zIndex, xIndex];

				Vector3 vertex = meshVertices [vertexIndex];
				// change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
				meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);

				vertexIndex++;
			}
		}

		// update the vertices in the mesh and update its properties
		mesh.SetVertices(meshVertices);
		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		this.meshFilter.mesh = mesh;
		// update the mesh collider
		this.meshCollider.sharedMesh = this.meshFilter.mesh;
	}
}

[System.Serializable]
public class TerrainType {
	public string name;
	public float minHeight;
	public float maxHeight;
	public float minMoisture;
	public float maxMoisture;
	public GameObject[] vegetation;
	public int numTiles;
}

public class TileData
{
	public float[,] heightMap;
	public float[,] moistureMap;
	public TerrainType[,] biomes;
	public Mesh mesh;

	public TileData(float[,] heightMap, float[,] moistureMap,
	  TerrainType[,] biomes, Mesh mesh)
	{
		this.heightMap = heightMap;
		this.moistureMap = moistureMap;
		this.biomes = biomes;
		this.mesh = mesh;
	}
}