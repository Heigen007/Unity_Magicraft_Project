using UnityEngine;

namespace Magicraft.World
{
    /// <summary>
    /// Один чанк процедурно генерируемой земли
    /// Представляет собой квадрат мира с травой/землёй
    /// </summary>
    public class TerrainChunk : MonoBehaviour
    {
        public Vector2Int ChunkCoord { get; private set; }
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        
        /// <summary>
        /// Инициализировать чанк
        /// </summary>
        public void Initialize(Vector2Int coord, Material material, int chunkSize, float noiseScale, int seed)
        {
            ChunkCoord = coord;
            
            // Создать компоненты
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            
            meshRenderer.material = material;
            meshRenderer.sortingLayerName = "Default";
            meshRenderer.sortingOrder = -1000; // Самый нижний слой (под всем)
            
            // Генерировать меш
            GenerateMesh(chunkSize, noiseScale, seed);
            
            // Позиция в мире (Z = 10 чтобы был позади всего)
            transform.position = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 10f);
        }
        
        /// <summary>
        /// Генерирует меш земли с цветовой вариацией через Perlin Noise
        /// </summary>
        private void GenerateMesh(int size, float noiseScale, int seed)
        {
            Mesh mesh = new Mesh();
            mesh.name = $"TerrainChunk_{ChunkCoord.x}_{ChunkCoord.y}";
            
            // Вершины и UV
            Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            Color[] colors = new Color[vertices.Length];
            
            int vertIndex = 0;
            for (int y = 0; y <= size; y++)
            {
                for (int x = 0; x <= size; x++)
                {
                    // Позиция вершины
                    vertices[vertIndex] = new Vector3(x, y, 0f);
                    uv[vertIndex] = new Vector2((float)x / size, (float)y / size);
                    
                    // Цвет из Perlin Noise
                    float worldX = ChunkCoord.x * size + x;
                    float worldY = ChunkCoord.y * size + y;
                    
                    // ОЧЕНЬ мелкие детали травы - максимальная частота
                    float noise1 = Mathf.PerlinNoise((worldX + seed * 0.1f) * noiseScale, (worldY + seed * 0.1f) * noiseScale);
                    float noise2 = Mathf.PerlinNoise((worldX + seed * 0.1f) * noiseScale * 2f, (worldY + seed * 0.1f) * noiseScale * 2f) * 0.5f;
                    float noiseValue = Mathf.Clamp01((noise1 + noise2) / 1.5f);
                    
                    // Хороший контраст для красивой травы
                    noiseValue = Mathf.Pow(noiseValue, 0.9f);
                    
                    // Красивые оттенки травы с заметной разницей
                    Color darkGrass = new Color(0.2f, 0.4f, 0.15f, 1f);    // Тёмно-зелёная трава
                    Color lightGrass = new Color(0.5f, 0.85f, 0.4f, 1f);   // Ярко-зелёная трава
                    
                    colors[vertIndex] = Color.Lerp(darkGrass, lightGrass, noiseValue);
                    
                    vertIndex++;
                }
            }
            
            // Треугольники (два треугольника на каждый квад)
            int[] triangles = new int[size * size * 6];
            int triIndex = 0;
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int bottomLeft = y * (size + 1) + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = (y + 1) * (size + 1) + x;
                    int topRight = topLeft + 1;
                    
                    // Первый треугольник
                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = bottomRight;
                    
                    // Второй треугольник
                    triangles[triIndex++] = bottomRight;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = topRight;
                }
            }
            
            // Применить к мешу
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
        
        /// <summary>
        /// Уничтожить чанк
        /// </summary>
        public void Destroy()
        {
            if (meshFilter != null && meshFilter.mesh != null)
            {
                Destroy(meshFilter.mesh);
            }
            Destroy(gameObject);
        }
    }
}
