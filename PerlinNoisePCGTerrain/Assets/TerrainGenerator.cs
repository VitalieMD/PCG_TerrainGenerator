using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int xSize = 10;
    [SerializeField] private int zSize = 10;

    [SerializeField] private float noiseScale = 0.03f;
    [SerializeField] private float heightMultiplier = 13;

    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f; //how much each octave contributes to the overall shape
    [SerializeField] private float lacunarity = 2f; //how much detail is added or removed at each octave
    private Mesh mesh;
    private Texture2D gradientTexture;
    [SerializeField] private Gradient terrainGradient;
    [SerializeField] private Material mat;

    [SerializeField] private float minGrass, maxGrass, minTree, maxTree;
    [SerializeField] private GameObject[] grass, trees, rocks;
    private int xOffset;
    private int zOffset;
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
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateVertices();
        GenerateTriangles();
        GradientToTexture();

        var transformPos = transform.position;
        minTerrainHeight = mesh.bounds.min.y + transformPos.y - 0.01f;
        maxTerrainHeight = mesh.bounds.max.y + transformPos.y + 0.01f;

        mat.SetTexture(Gradient, gradientTexture);

        mat.SetFloat(MinHeight, minTerrainHeight);
        mat.SetFloat(MaxHeight, maxTerrainHeight);
        InstGrass();
    }

    private void GradientToTexture()
    {
        gradientTexture = new Texture2D(1, 100);
        var pixelColors = new Color[100];

        for (var i = 0; i < 100; i++)
        {
            pixelColors[i] = terrainGradient.Evaluate((float)i / 100);
        }

        gradientTexture.SetPixels(pixelColors);
        gradientTexture.Apply();
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
                meshTriangles[corner + 0] = vert + 0;
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
                var xPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1 + xSize], Random.Range(0.3f, 0.85f));
                var grassHeight = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, xPosLerp.y);
                if (xPosLerp.y / heightMultiplier - 0.01f < maxGrass & xPosLerp.y / heightMultiplier - 0.01f > minGrass)
                {
                    Instantiate(grass[Random.Range(0, grass.Length)], xPosLerp, Quaternion.identity);
                }

                if (xPosLerp.y / heightMultiplier - 0.01f < maxTree & xPosLerp.y / heightMultiplier - 0.01f > minTree)
                {
                    if (Random.Range(0, 10) > 9)
                        Instantiate(trees[Random.Range(0, trees.Length)], xPosLerp, Quaternion.identity);
                }
            }

            if (vert + 1 >= vertices.Length) continue;
            if (!(vertices[vert].x < vertices[vert + 1].x)) continue;
            var zPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1], Random.Range(0.3f, 0.85f));
            if (zPosLerp.y / heightMultiplier - 0.01f < maxGrass & zPosLerp.y / heightMultiplier - 0.01f > minGrass)
            {
                Instantiate(grass[Random.Range(0, grass.Length)], zPosLerp, Quaternion.identity);
            }

            zPosLerp = Vector3.Lerp(vertices[vert], vertices[vert + 1], Random.Range(0.3f, 0.85f));
            if (zPosLerp.y / heightMultiplier - 0.01f < maxTree & zPosLerp.y / heightMultiplier - 0.01f > minTree)
            {
                if (Random.Range(0, 10) > 8)
                    Instantiate(trees[Random.Range(0, trees.Length)], zPosLerp, Quaternion.identity);
            }
        }
    }
}