using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;


//Parts adapted from Game Dev Academy https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
//Parts adapted from Catlike Coding https://catlikecoding.com/unity/tutorials/procedural-grid/
public class NavMeshTest : MonoBehaviour
{
	public NoiseMapGeneration noiseMapGeneration;

	public TreeGeneration treeGeneration;

	public NavMeshSurface surface;

	[SerializeField]
	private MeshRenderer meshRenderer;
	[SerializeField]
	private MeshFilter meshFilter;
	[SerializeField]
	private MeshCollider meshCollider;

	private Mesh mesh;
	[SerializeField]
	private int zSize = 200;
	[SerializeField]
	private int xSize = 200;
	private int distPerVertex = 5;
	float[,] heightMap;

	public Wave[] heightWaves;

	public float heightMultiplier;

	void Start()
	{
		createTile();
	}

	public void SetParams(float heightMultiplier)
	{
		this.heightMultiplier = heightMultiplier;
	}


	public void createTile()
	{

		heightMap = new float[zSize + 1, xSize + 1];
		for (int i = 0; i < zSize + 1; i++)
        {
			for (int j = 0; j < xSize + 1; j++)
            {
				heightMap[i, j] = 0;
            }
        }
		GenerateGrid();
		setTriangles();
		generateMaps();
		GenerateTile();
		surface.BuildNavMesh();
	}





	private void GenerateGrid()
	{
		meshFilter.mesh = mesh = new Mesh();
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
		List<int> list = new List<int>();
		for (int ti = 0, vi = 0, z = 0; z < zSize; z++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				list.Add(vi);
				list.Add(vi + zSize + 1);
				list.Add(vi + 1);
				list.Add(vi + 1);
				list.Add(vi + zSize + 1);
				list.Add(vi + zSize + 2);
			}
		}

		mesh.triangles=list.ToArray();
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
        heightMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, heightWaves);
        //float[,] mountainNoise = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, mountainWaves);
        //noiseMapGeneration.removeHeightFunction();
        //moistureMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, moistureWaves);
        //heatMap = noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, distPerVertex, offsetX, offsetZ, heatWaves);
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
}


