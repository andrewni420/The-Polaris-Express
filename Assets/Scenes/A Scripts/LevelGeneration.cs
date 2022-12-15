using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class LevelGeneration : MonoBehaviour {

	[SerializeField]
	private int mapWidthInTiles, mapDepthInTiles;

	public TreeGeneration treeGeneration;

	public Voronoi voronoiDiagram;

	public LevelData data;

	//public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
	public NavMeshSurface surface;

	[SerializeField]
	private int tileZSize = 20;
	[SerializeField]
	private int tileXSize = 20;
	[SerializeField]
	private int distPerVertex = 1;
	[SerializeField]
	private int randomSeed = 1;
	public bool useSeed = false;

	[SerializeField]
	private float heightMultiplier;

	[SerializeField]
	private GameObject gatePrefab;

	public float mtnThickness = 10;

	public float groundMountainRatio;
	[SerializeField]
	private GameObject tilePrefab;

	void Start() {
		if (!useSeed) randomSeed = (int)System.DateTime.Now.Ticks;
		voronoiDiagram.setParams(mtnThickness, groundMountainRatio, falloff, mapWidthInTiles*(tileXSize+1), randomSeed);
		GenerateMap ();
		makeGates();
		//foreach (NavMeshSurface s in surfaces)
  //      {
		//	s.BuildNavMesh();
  //      }
		surface.BuildNavMesh();
		
	}

	void GenerateMap() {
		LevelData levelData = new LevelData(tileZSize, tileXSize, mapDepthInTiles, mapWidthInTiles);

		// for each Tile, instantiate a Tile in the correct position
		for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
				// calculate the tile position based on the X and Z indices
				Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * distPerVertex * tileZSize,
					this.gameObject.transform.position.y,
					this.gameObject.transform.position.z + zTileIndex * distPerVertex * tileXSize);
				// instantiate a new Tile
				GameObject tile = Instantiate (tilePrefab, tilePosition, Quaternion.identity, transform) as GameObject;
				TileGeneration generator = tile.GetComponent<TileGeneration>();
				//surfaces.Add(generator.surface);
				generator.SetParams(heightMultiplier);
				generator.levelGenerator = this;
				generator.noiseMapGeneration.setHeightFunction((float noise, float z, float x)=>heightFunction(noise, (z+zTileIndex)/mapDepthInTiles, (x+xTileIndex)/mapWidthInTiles));
				generator.setSize(tileZSize, tileXSize, distPerVertex, mtnThickness/(tileXSize+1)/mapWidthInTiles);
				generator.toUnitSquare = (int z, int x) => (((float)z/tileZSize+zTileIndex)/mapDepthInTiles, ((float)x/tileXSize+xTileIndex)/mapWidthInTiles);
				generator.setIndex(zTileIndex, xTileIndex);
				generator.initializeWaves(randomSeed);
				generator.setLevels();
				generator.createTile();
				levelData.AddTileData(generator.getData(),zTileIndex,xTileIndex);
			}
		}


		data = levelData;
		treeGeneration.generateTrees(distPerVertex, levelData);
	}

	float normalHeight(float noise, float z, float x)
    {
		return groundMountainRatio * noise;
	}

	float mountainHeight(float noise, float z, float x)
    {
		int levelSize = (tileXSize + 1) * mapWidthInTiles;
		float mtn = mtnThickness / levelSize;
		float gmr = groundMountainRatio;
		float zdist = Mathf.Abs(z - 0.5f);
		float xdist = Mathf.Abs(x - 0.5f);

		//if (zdist < 0.4 && xdist < 0.4) return gmr * noise;

		//Use L-infinity norm to make equidistant surfaces in the shape of squares
		//Linear function to go from 0 at 0.40 to 1 at 0.47 and back down.
		//Positioning max at 0.46 prevents an awkward patch of 0 height terrain at edge of world;
		float offset = mtn * 6 / 5;
		float s = (offset - Mathf.Abs(0.5f-4*mtn/5 - Mathf.Max(zdist, xdist))) / offset;

		//Exponential function to slowly transition from 0 to 0.6 prevents "ground level" biomes from going too high on the mountains
		//Linearly increase amplitude of noise to smoothly transition into mountain peaks
		return falloff(s, noise);
	}


	float heightFunction(float noise, float z, float x)
    {
		return Mathf.Max(voronoiDiagram.heightFunction(noise, z, x), mountainHeight(noise, z, x));
    }

	public float falloff(float distance, float noise)
	{
		distance = Mathf.Max(distance, 0);
		return Mathf.Pow(7, distance) / 10 - 0.1f + (groundMountainRatio + (0.4f - groundMountainRatio) * distance) * noise;
	}

	public (float z, float x) getCoordinates(int zIndex, int xIndex, int depthIndex, int widthIndex)
    {
		return ((zIndex / (float)(tileZSize+1) + depthIndex) / (float)mapDepthInTiles, (xIndex / (float)(tileXSize+1) + widthIndex) / (float)mapWidthInTiles);
    }

	//Combines a value m1 from heightNoise and a value m2 from mountainNoise into a value for heightMap depending on the coordinates
	public float combineMaps(float m1, float m2, float x, float y)
	{
		float mtn = mtnThickness / (tileXSize + 1) / mapWidthInTiles;
		//L-infinity distance from center. Set Linf=0 when distance = 0.4, the transition from normal to mountain terrain
		float Linf = Mathf.Max(Mathf.Abs(x - 0.5f), Mathf.Abs(y - 0.5f)) - (0.5f - 2 * mtn);

		//L2 distance from nearest edge
		float L2 = voronoiDiagram.closestEdgeDistance(x, y) - mtn;


		//Sigmoid to smoothly transition from hill generation to normal generation to mountainous generation
		float s1 = 1 - sigmoid(100 * Linf);
		float s2 = sigmoid(100 * L2);

		return (1 - s1 * s2) * m2 + s1 * s2 * m1;
	}

	float sigmoid(float x) { return 1 / (1 + Mathf.Exp(-x)); }

	public void makeGates()
	{
		float levelSize = mapDepthInTiles * tileZSize * distPerVertex;
		foreach (Bisector b in voronoiDiagram.bisectors)
		{
			Vector3 pos = new Vector3(b.fogGate.y * levelSize,heightMultiplier/3, b.fogGate.x * levelSize);
			Vector3 dir = (b.start - b.end).normalized;

			GameObject obj = Instantiate(gatePrefab, pos, Quaternion.LookRotation(new Vector3(dir.x, 0,-dir.y)));

			float width = mtnThickness * distPerVertex*2;
			float height = heightMultiplier * 2 / 3;
			float depth = width / 5;
			obj.transform.localScale = new Vector3(width, height, depth);

			b.gateObject = obj;

		}
	}

	public (int z, int x) levelCoordSize()
    {
		return (mapDepthInTiles * tileZSize * distPerVertex, mapWidthInTiles * tileXSize * distPerVertex);
    }
}

/********* Level Data Class **********/
public class LevelData
{
	private int tileDepthInVertices, tileWidthInVertices;
	public TileData[,] tileData;

	public LevelData(int tileDepthInVertices, int tileWidthInVertices, int levelDepthInTiles, int levelWidthInTiles)
	{
		// build the tilesData matrix based on the level depth and width
		tileData = new TileData[levelDepthInTiles,levelWidthInTiles];
		this.tileDepthInVertices = tileDepthInVertices;
		this.tileWidthInVertices = tileWidthInVertices;
	}
	public (int z, int x) getTileSize() { return (tileDepthInVertices, tileWidthInVertices); }
	public (int z, int x) getSizeInTiles() { return (tileData.GetLength(0), tileData.GetLength(1)); }
	public (int z, int x) getSizeInVertices() { return (tileData.GetLength(0)* tileDepthInVertices, tileData.GetLength(1)* tileWidthInVertices); }
	public void AddTileData(TileData data, int tileZIndex, int tileXIndex)
	{
		// save the TileData in the corresponding coordinate
		data.treeMap = new float[tileDepthInVertices, tileWidthInVertices];
		tileData[tileZIndex, tileXIndex] = data;
	}

	public TileCoordinate ConvertToTileCoordinate(int zIndex, int xIndex)
	{
		// the tile index is calculated by dividing the index by the number of tiles in that axis
		int tileZIndex = (int)Mathf.Floor((float)zIndex / (float)tileDepthInVertices);
		int tileXIndex = (int)Mathf.Floor((float)xIndex / (float)tileWidthInVertices);
		// the coordinate index is calculated by getting the remainder of the division above
		// we also need to translate the origin to the bottom left corner
		int coordinateZIndex = (zIndex % tileDepthInVertices);
		int coordinateXIndex = (xIndex % tileDepthInVertices);
		TileCoordinate tileCoordinate = new TileCoordinate(tileZIndex, tileXIndex, coordinateZIndex, coordinateXIndex);
		return tileCoordinate;
	}

	public void setTreeMap(float value, int zIndex, int xIndex)
    {
		TileCoordinate coord = ConvertToTileCoordinate(zIndex, xIndex);
		TileData data = tileData[coord.tileZIndex, coord.tileXIndex];
		float[,] treeMap = data.treeMap;
		treeMap[coord.coordinateZIndex, coord.coordinateXIndex] = value;
		//tileData[coord.tileZIndex,coord.tileXIndex].treeMap[coord.coordinateZIndex,coord.coordinateXIndex]= value;
	}
	public float getTreeMap(int zIndex, int xIndex){return getFloat("tree", zIndex, xIndex);}
	public void setTreeMap(float[,]map, int zIndex, int xIndex)
    {
		tileData[zIndex, xIndex].treeMap = map;
    }
	public TerrainType getBiome(int zIndex, int xIndex)
    {
		TileCoordinate coord = ConvertToTileCoordinate(zIndex, xIndex);
		return tileData[coord.tileZIndex, coord.tileXIndex].biomes[coord.coordinateZIndex, coord.coordinateXIndex];
	}
	public float getHeight(int zIndex, int xIndex){return getFloat("height", zIndex, xIndex);}

	public float getFloat(string map, int zIndex, int xIndex)
    {
		TileCoordinate coord = ConvertToTileCoordinate(zIndex, xIndex);
        switch (map) {
			case "height":
				return tileData[coord.tileZIndex, coord.tileXIndex].heightMap[coord.coordinateZIndex, coord.coordinateXIndex];
			case "moisture":
				return tileData[coord.tileZIndex, coord.tileXIndex].moistureMap[coord.coordinateZIndex, coord.coordinateXIndex];
			case "tree":
				return tileData[coord.tileZIndex, coord.tileXIndex].treeMap[coord.coordinateZIndex, coord.coordinateXIndex];
			default:
				return tileData[coord.tileZIndex, coord.tileXIndex].heightMap[coord.coordinateZIndex, coord.coordinateXIndex];
		}
	}

	public Vector3 getVertex(int zIndex, int xIndex)
    {
		TileCoordinate coord = ConvertToTileCoordinate(zIndex, xIndex);
		return tileData[coord.tileZIndex, coord.tileXIndex].mesh.vertices[coord.coordinateZIndex * (getTileSize().x+1) + coord.coordinateXIndex];
	}

	public bool project(Vector3 point, out Vector3 projection, float maxDistance=1000)
    {
		projection = new Vector3(-1, -1, -1);
		RaycastHit hit;
		if (Physics.Raycast(point,Vector3.down, out hit, maxDistance, 64))
        {
			projection = hit.point;
			return true;
        }
		else if (Physics.Raycast(point, Vector3.up, out hit, maxDistance, 64))
        {
			projection = hit.point;
			return true;
        }
		return false;
    }

	

}

public class TileCoordinate
{
	public int tileZIndex;
	public int tileXIndex;
	public int coordinateZIndex;
	public int coordinateXIndex;
	public TileCoordinate(int tileZIndex, int tileXIndex, int coordinateZIndex, int coordinateXIndex)
	{
		this.tileZIndex = tileZIndex;
		this.tileXIndex = tileXIndex;
		this.coordinateZIndex = coordinateZIndex;
		this.coordinateXIndex = coordinateXIndex;
	}
}
