using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;


//Parts adapted from Game Dev Academy https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
//Parts adapted from Catlike Coding https://catlikecoding.com/unity/tutorials/procedural-grid/
public class NavMeshTest2 : MonoBehaviour
{

	[SerializeField]
	public NoiseMapGeneration noiseMapGeneration;

	public delegate (float x, float y) function(int zIndex, int xIndex);
	public function toUnitSquare;

	public Voronoi voronoiDiagram;

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
	[SerializeField]
	private int zSize = 200;
	[SerializeField]
	private int xSize = 200;
	public int distPerVertex = 5;
	float[,] heightMap;
	float[,] moistureMap;
	float[,] heatMap;
	TerrainType[,] biomes;

	//Biome moisture levels determined using ComputeLevelCurves.cs
	[SerializeField]
	private GeneralTerrain Terrains;

	public PDF PerlinPDF;

	public float heightMultiplier;

	public float mtn;

	[SerializeField]
	public float groundMountainRatio;


	//Use perlin waves to generate fractal noise
	[SerializeField]
	private Wave[] heightWaves;
	[SerializeField]
	private Wave[] mountainWaves;
	[SerializeField]
	private Wave[] moistureWaves;
	[SerializeField]
	private Wave[] heatWaves;

	void Start()
	{
		noiseMapGeneration.setHeightFunction(heightFunction);
		voronoiDiagram.setParams(mtn*(xSize+1), groundMountainRatio, falloff, (xSize + 1), 1);
		toUnitSquare = (int z, int x) => ((float)z / zSize, (float)x / xSize);
		setLevels();
		createTile();
		surface.BuildNavMesh();
	}

	float heightFunction(float noise, float z, float x)
	{
		return Mathf.Max(voronoiDiagram.heightFunction(noise, z, x), mountainHeight(noise, z, x));
	}

	float mountainHeight(float noise, float z, float x)
	{
		float gmr = groundMountainRatio;
		float zdist = Mathf.Abs(z - 0.5f);
		float xdist = Mathf.Abs(x - 0.5f);

		//if (zdist < 0.4 && xdist < 0.4) return gmr * noise;

		//Use L-infinity norm to make equidistant surfaces in the shape of squares
		//Linear function to go from 0 at 0.40 to 1 at 0.47 and back down.
		//Positioning max at 0.46 prevents an awkward patch of 0 height terrain at edge of world;
		float offset = mtn * 6 / 5;
		float s = (offset - Mathf.Abs(0.5f - 4 * mtn / 5 - Mathf.Max(zdist, xdist))) / offset;

		//Exponential function to slowly transition from 0 to 0.6 prevents "ground level" biomes from going too high on the mountains
		//Linearly increase amplitude of noise to smoothly transition into mountain peaks
		return falloff(s, noise);
	}

	public float falloff(float distance, float noise)
	{
		distance = Mathf.Max(distance, 0);
		return Mathf.Pow(7, distance) / 10 - 0.1f + (groundMountainRatio + (0.4f - groundMountainRatio) * distance) * noise;
	}

	public void setLevels()
	{
		Terrains.initLevels(PerlinPDF);
		Terrains.initTerrains();

		Terrains.initRenderer(meshRenderer);
	}

	public void createTile()
	{
		Terrains.resetNumTiles();

		heightMap = new float[zSize + 1, xSize + 1];
		biomes = new TerrainType[zSize + 1, xSize + 1];
		GenerateGrid();
		generateMaps();
		setTriangles();
		GenerateTile();
		GenerateTrees();
	}

	void GenerateTrees()
	{
		LevelData levelData = new LevelData(zSize, xSize, 1, 1);
		TileData data = getData();
		levelData.AddTileData(data, 0, 0);

		Vector3 tileSize = meshRenderer.bounds.size;
		int tileWidth = (int)tileSize.x;
		int tileDepth = (int)tileSize.z;
		// calculate the number of vertices of the tile in each axis using its mesh
		int tileDepthInVertices = (int)Mathf.Sqrt(mesh.vertices.Length);
		int tileWidthInVertices = tileDepthInVertices;

		float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;

		treeGenerator.generateTrees(distPerVertex, levelData);
	}

	public TileData getData() { return new TileData(heightMap, moistureMap, heatMap, biomes, mesh); }

	public void initializeWaves(int seed)
	{
		int length = heightWaves.Length + mountainWaves.Length + mountainWaves.Length + heatWaves.Length;
		Random.InitState(seed);
		int curSeed = 0;
		int[] seeds = new int[length];
		for (int i = 0; i < seeds.Length; i++)
		{
			//guarantees no overlap of waves with frequency up to 10
			seeds[i] = Random.Range(curSeed + 10 * xSize, curSeed + 100 * xSize);
			curSeed = seeds[i];
		}

		int j = 0;
		foreach (Wave w in heightWaves) w.seed = seeds[j++];
		foreach (Wave w in mountainWaves) w.seed = seeds[j++];
		foreach (Wave w in moistureWaves) w.seed = seeds[j++];
		foreach (Wave w in heatWaves) w.seed = seeds[j++];

	}



	private void GenerateGrid()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		Vector3[] vertices = new Vector3[(zSize + 1) * (xSize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				vertices[i] = new Vector3(distPerVertex * x, 0, distPerVertex * z);
				uv[i] = new Vector2((float)z / zSize, (float)x / xSize);
			}
		}
		mesh.vertices = vertices;
		mesh.uv = uv;

	}

	void setTriangles()
	{

		for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				List<int> list = biomes[z, x].triangles;
				list.Add(vi);
				list.Add(vi + zSize + 1);
				list.Add(vi + 1);
				list.Add(vi + 1);
				list.Add(vi + zSize + 1);
				list.Add(vi + zSize + 2);
			}
		}

		Terrains.initMesh(mesh);
	}


	void generateMaps()
	{
		// calculate tile depth and width based on the mesh vertices
		Vector3[] meshVertices = this.meshFilter.mesh.vertices;
		int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
		int tileWidth = tileDepth;

		// calculate the offsets based on the tile position
		float offsetX = transform.position.x;
		float offsetZ = transform.position.z;

		// generate a heightMap using noise
		float[,] heightNoise = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, heightWaves);
		float[,] mountainNoise = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, mountainWaves);
		noiseMapGeneration.removeHeightFunction();
		moistureMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, moistureWaves);
		heatMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, heatWaves);

		//Combine height and mountain noise into heightmap
		for (int zIndex = 0; zIndex < tileDepth; zIndex++)
		{
			for (int xIndex = 0; xIndex < tileWidth; xIndex++)
			{
				(float x, float y) unit = toUnitSquare(zIndex, xIndex);
				heightMap[zIndex, xIndex] = combineMaps(heightNoise[zIndex, xIndex], mountainNoise[zIndex, xIndex], unit.x, unit.y);
				biomes[zIndex, xIndex] = ChooseTerrainType(
					heightMap[zIndex, xIndex],
					moistureMap[zIndex, xIndex],
					heatMap[zIndex, xIndex],
					voronoiDiagram.section(unit.x, unit.y));
				biomes[zIndex, xIndex].numTiles++;
			}
		}
	}

	public float combineMaps(float m1, float m2, float x, float y)
	{
		//L-infinity distance from center. Set Linf=0 when distance = 0.4, the transition from normal to mountain terrain
		float Linf = Mathf.Max(Mathf.Abs(x - 0.5f), Mathf.Abs(y - 0.5f)) - (0.5f - 2 * mtn);

		//L2 distance from nearest edge
		float L2 = voronoiDiagram.closestEdgeDistance(x, y) - mtn;


		//Sigmoid to smoothly transition from hill generation to normal generation to mountainous generation
		float s1 = 1 - sigmoid(100 * Linf);
		float s2 = sigmoid(100 * L2);

		return (1 - s1 * s2) * m2 + s1 * s2 * m1;
	}

	void GenerateTile()
	{

		// build a Texture2D from the height map
		foreach (Material material in this.meshRenderer.materials)
		{
			material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		}

		// update the tile mesh vertices according to the height map
		UpdateMeshVertices(heightMap);

	}


	TerrainType ChooseTerrainType(float height, float moisture, float heat, int section)
	{

		return (TerrainType)Terrains.chooseTerrain(new Dictionary<string, float>() {
			{"height", height},
			{"moisture", moisture},
			{"heat", heat},
			{"section", section }
		});
	}

	private void UpdateMeshVertices(float[,] heightMap)
	{
		this.meshFilter.mesh = mesh;

		int tileDepth = heightMap.GetLength(0);
		int tileWidth = heightMap.GetLength(1);

		Vector3[] meshVertices = this.meshFilter.mesh.vertices;

		// iterate through all the heightMap coordinates, updating the vertex index
		int vertexIndex = 0;
		for (int zIndex = 0; zIndex < tileDepth; zIndex++)
		{
			for (int xIndex = 0; xIndex < tileWidth; xIndex++)
			{
				float height = heightMap[zIndex, xIndex];

				Vector3 vertex = meshVertices[vertexIndex];
				// change the vertex Y coordinate, proportional to the height value. The height value is evaluated by the heightCurve function, in order to correct it.
				meshVertices[vertexIndex] = new Vector3(vertex.x, height * this.heightMultiplier, vertex.z);

				vertexIndex++;
			}
		}

		// update the vertices in the mesh and update its properties
		mesh.SetVertices(meshVertices);
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

		this.meshFilter.mesh = mesh;
		// update the mesh collider
		this.meshCollider.sharedMesh = this.meshFilter.mesh;
	}
	float sigmoid(float x) { return 1 / (1 + Mathf.Exp(-x)); }
}


