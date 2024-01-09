using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;

    [SerializeField] private float noiseScale = 0.03f;
    [SerializeField] private float heightMultiplier = 2;

    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f; 
    
    private int xOffset;
    private int zOffset;
    
    private Mesh mesh;
    private Texture2D gradTexture2D;
    [SerializeField] private Gradient terrainGradient;
    [SerializeField] private Material mat;

    [SerializeField] private int grassMultiplier = 1;
    [SerializeField] private float minGrass, maxGrass, minTree, maxTree;
    [SerializeField] private GameObject[] grass, trees, rocks;

    private Vector3[] vertices;

    private float minTerrainHeight;
    private float maxTerrainHeight;

    private static readonly int Gradient = Shader.PropertyToID("gradient");
    private static readonly int MinHeight = Shader.PropertyToID("minHeight");
    private static readonly int MaxHeight = Shader.PropertyToID("maxHeight");


    private void Awake()
    {
        xOffset = Random.Range(1, 10000);
        zOffset = Random.Range(1, 10000);
    }

    private void Start()
    {
        xSize = UI_Controller._instance.width;
        zSize = UI_Controller._instance.length;
        heightMultiplier = UI_Controller._instance.height;
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateVertices();
        GenerateTriangles();
        GradientToTexture();

        var transformPos = transform.position;
        minTerrainHeight = mesh.bounds.min.y + transformPos.y - 0.01f;
        maxTerrainHeight = mesh.bounds.max.y + transformPos.y + 0.01f;

        mat.SetTexture(Gradient, gradTexture2D);

        mat.SetFloat(MinHeight, minTerrainHeight);
        mat.SetFloat(MaxHeight, maxTerrainHeight);
        InstGrass();
        gameObject.AddComponent(typeof(MeshCollider));
    }


    private void GradientToTexture()
    {
        gradTexture2D = new Texture2D(1, 100);
        var pixelColors = new Color[100];

        for (var i = 0; i < 100; i++)
        {
            pixelColors[i] = terrainGradient.Evaluate((float)i / 100);
        }

        gradTexture2D.SetPixels(pixelColors);
        gradTexture2D.Apply();
    }

    private void GenerateVertices()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        var i = 0;
        for (var z = 0; z <= zSize; z++)
        {
            for (var x = 0; x <= xSize; x++)
            {
                float yPos = 0;
                for (var o = 0; o < octaves; o++)
                {
                    var frequency = Mathf.Pow(lacunarity, o);
                    var amplitude = Mathf.Pow(persistence, o);
                    yPos += Mathf.PerlinNoise((x + xOffset) * noiseScale * frequency,
                        (z + zOffset) * noiseScale * frequency) * amplitude;
                }

                yPos *= heightMultiplier;
                vertices[i] = new Vector3(x, yPos, z);

                i++;
            }
        }
    }

    private void GenerateTriangles()
    {
        var meshTriangles = new int[xSize * zSize * 6];

        var vert = 0;
        var corner = 0;

        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                meshTriangles[corner] = vert;
                meshTriangles[corner + 1] = vert + xSize + 1;
                meshTriangles[corner + 2] = vert + 1;

                meshTriangles[corner + 3] = vert + 1;
                meshTriangles[corner + 4] = vert + xSize + 1;
                meshTriangles[corner + 5] = vert + xSize + 2;

                vert++;
                corner += 6;
            }

            vert++;
        }


        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();
    }

    private void InstGrass()
    {
        for (var vert = 0; vert < vertices.Length; vert++)
        {
            if (vert + 1 + xSize < vertices.Length)
            {
                var xPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1 + xSize], Random.Range(0f, 1f));
                for (int i = 0; i < grassMultiplier; i++)
                {
                    if (xPosLerp.y / heightMultiplier  < maxGrass &
                        xPosLerp.y / heightMultiplier> minGrass)
                    {
                        Instantiate(grass[Random.Range(0, grass.Length)], xPosLerp, Quaternion.identity);
                    }
                }

                if (xPosLerp.y / heightMultiplier < maxTree & xPosLerp.y / heightMultiplier  > minTree)
                {
                    if (Random.Range(0, 10) > 9)
                        Instantiate(trees[Random.Range(0, trees.Length)], xPosLerp, Quaternion.identity);
                }
            }
            if (vert + 1 >= vertices.Length) continue;
            if (!(vertices[vert].x < vertices[vert + 1].x)) continue;
            var zPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1], Random.Range(0f, 1f));
            for (int i = 0; i < grassMultiplier; i++)
            {
                if (zPosLerp.y / heightMultiplier < maxGrass & zPosLerp.y / heightMultiplier  > minGrass)
                {
                    Instantiate(grass[Random.Range(0, grass.Length)], zPosLerp, Quaternion.identity);
                }
            }


            zPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1], Random.Range(0f, 1f));
            if (zPosLerp.y / heightMultiplier  < maxTree & zPosLerp.y / heightMultiplier  > minTree)
            {
                if (Random.Range(0, 10) > 8)
                    Instantiate(trees[Random.Range(0, trees.Length)], zPosLerp, Quaternion.identity);
            }
        }
    }
}