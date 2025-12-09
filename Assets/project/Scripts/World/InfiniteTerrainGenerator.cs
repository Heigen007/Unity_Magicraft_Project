using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.World
{
    /// <summary>
    /// Генератор бесконечного мира вокруг игрока
    /// Создаёт и удаляет чанки земли по мере движения
    /// </summary>
    public class InfiniteTerrainGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Размер одного чанка в юнитах (больше = меньше швов)")]
        [SerializeField] private int chunkSize = 50;
        
        [Tooltip("Радиус видимости в чанках")]
        [SerializeField] private int viewDistance = 2;
        
        [Tooltip("Масштаб Perlin Noise (больше = мельче детали)")]
        [SerializeField] private float noiseScale = 1.5f;
        
        [Tooltip("Seed для генерации (одинаковый seed = одинаковый мир)")]
        [SerializeField] private int seed = 12345;
        
        [Header("References")]
        [Tooltip("Игрок (для отслеживания позиции)")]
        [SerializeField] private Transform player;
        
        [Tooltip("Материал для земли")]
        [SerializeField] private Material terrainMaterial;
        
        [Header("Debug")]
        [Tooltip("Показывать отладочную информацию")]
        [SerializeField] private bool showDebug = true;
        
        // Активные чанки
        private Dictionary<Vector2Int, TerrainChunk> activeChunks = new Dictionary<Vector2Int, TerrainChunk>();
        
        // Последняя позиция игрока (в координатах чанков)
        private Vector2Int lastPlayerChunk;
        
        private void Start()
        {
            // Найти игрока если не назначен
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
            
            // Создать материал если не назначен
            if (terrainMaterial == null)
            {
                // Используем кастомный shader для vertex colors
                Shader shader = Shader.Find("Custom/VertexColor");
                if (shader == null)
                {
                    Debug.LogWarning("[Terrain] Custom/VertexColor shader не найден! Земля будет белой.");
                    shader = Shader.Find("Unlit/Color");
                }
                
                terrainMaterial = new Material(shader);
            }
            
            // Генерировать начальные чанки
            if (player != null)
            {
                lastPlayerChunk = GetChunkCoord(player.position);
                UpdateChunks();
            }
        }
        
        private void Update()
        {
            if (player == null) return;
            
            // Проверить, перешёл ли игрок в новый чанк
            Vector2Int currentChunk = GetChunkCoord(player.position);
            
            if (currentChunk != lastPlayerChunk)
            {
                lastPlayerChunk = currentChunk;
                UpdateChunks();
            }
        }
        
        /// <summary>
        /// Обновить чанки вокруг игрока
        /// </summary>
        private void UpdateChunks()
        {
            // Определить какие чанки должны быть видимы
            HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();
            
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                for (int x = -viewDistance; x <= viewDistance; x++)
                {
                    Vector2Int coord = lastPlayerChunk + new Vector2Int(x, y);
                    chunksToKeep.Add(coord);
                    
                    // Создать чанк если его нет
                    if (!activeChunks.ContainsKey(coord))
                    {
                        CreateChunk(coord);
                    }
                }
            }
            
            // Удалить далёкие чанки
            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var coord in activeChunks.Keys)
            {
                if (!chunksToKeep.Contains(coord))
                {
                    toRemove.Add(coord);
                }
            }
            
            foreach (var coord in toRemove)
            {
                RemoveChunk(coord);
            }
            
            if (showDebug)
            {
                Debug.Log($"[Terrain] Active chunks: {activeChunks.Count}, Player chunk: {lastPlayerChunk}");
            }
        }
        
        /// <summary>
        /// Создать новый чанк
        /// </summary>
        private void CreateChunk(Vector2Int coord)
        {
            GameObject chunkObj = new GameObject($"Chunk_{coord.x}_{coord.y}");
            chunkObj.transform.SetParent(transform);
            chunkObj.layer = LayerMask.NameToLayer("Default");
            
            TerrainChunk chunk = chunkObj.AddComponent<TerrainChunk>();
            chunk.Initialize(coord, terrainMaterial, chunkSize, noiseScale, seed);
            
            activeChunks[coord] = chunk;
        }
        
        /// <summary>
        /// Удалить чанк
        /// </summary>
        private void RemoveChunk(Vector2Int coord)
        {
            if (activeChunks.TryGetValue(coord, out TerrainChunk chunk))
            {
                chunk.Destroy();
                activeChunks.Remove(coord);
            }
        }
        
        /// <summary>
        /// Получить координату чанка из мировой позиции
        /// </summary>
        private Vector2Int GetChunkCoord(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / chunkSize);
            int y = Mathf.FloorToInt(worldPos.y / chunkSize);
            return new Vector2Int(x, y);
        }
        
        /// <summary>
        /// Очистить все чанки
        /// </summary>
        public void ClearAllChunks()
        {
            foreach (var chunk in activeChunks.Values)
            {
                chunk.Destroy();
            }
            activeChunks.Clear();
        }
        
        private void OnDestroy()
        {
            ClearAllChunks();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (player == null) return;
            
            // Показать границы активной зоны
            Vector2Int playerChunk = GetChunkCoord(player.position);
            
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                playerChunk.x * chunkSize + chunkSize * 0.5f,
                playerChunk.y * chunkSize + chunkSize * 0.5f,
                0f
            );
            Vector3 size = new Vector3(
                (viewDistance * 2 + 1) * chunkSize,
                (viewDistance * 2 + 1) * chunkSize,
                1f
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}
