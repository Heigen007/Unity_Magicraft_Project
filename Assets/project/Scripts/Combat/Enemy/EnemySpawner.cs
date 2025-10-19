using UnityEngine;

namespace Magicraft.Combat.Enemy
{
    /// <summary>
    /// Спавнер врагов волнами
    /// Спавнит врагов вокруг игрока на определённом расстоянии
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Интервал между спавнами (секунды)")]
        [SerializeField] private float spawnInterval = 2f;

        [Tooltip("Количество врагов за один спавн")]
        [SerializeField] private int enemiesPerSpawn = 1;

        [Tooltip("Максимальное количество врагов одновременно")]
        [SerializeField] private int maxEnemies = 20;

        [Header("Spawn Area")]
        [Tooltip("Минимальная дистанция от игрока")]
        [SerializeField] private float minSpawnDistance = 5f;

        [Tooltip("Максимальная дистанция от игрока")]
        [SerializeField] private float maxSpawnDistance = 10f;

        [Header("References")]
        [Tooltip("Цель для врагов (обычно игрок)")]
        [SerializeField] private Transform target;

        [Tooltip("Пул врагов")]
        [SerializeField] private EnemyPool enemyPool;

        // Состояние
        private float spawnTimer;
        private int currentEnemyCount;
        private bool isSpawning = true;

        private void Start()
        {
            // Найти игрока автоматически
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }

            // Найти EnemyPool если не назначен
            if (enemyPool == null)
            {
                enemyPool = EnemyPool.Instance;
            }

            spawnTimer = spawnInterval;
        }

        private void Update()
        {
            if (!isSpawning) return;
            if (target == null) return;
            if (enemyPool == null) return;

            // Таймер спавна
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                TrySpawnWave();
                spawnTimer = spawnInterval;
            }
        }

        /// <summary>
        /// Попытка заспавнить волну врагов
        /// </summary>
        private void TrySpawnWave()
        {
            // Проверить лимит
            if (currentEnemyCount >= maxEnemies)
            {
                return;
            }

            // Заспавнить врагов
            int toSpawn = Mathf.Min(enemiesPerSpawn, maxEnemies - currentEnemyCount);

            for (int i = 0; i < toSpawn; i++)
            {
                SpawnEnemy();
            }
        }

        /// <summary>
        /// Заспавнить одного врага
        /// </summary>
        private void SpawnEnemy()
        {
            // Случайная позиция вокруг игрока
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 spawnPosition = target.position + (Vector3)(randomDirection * randomDistance);
            spawnPosition.z = 0f;

            // Получить врага из пула
            EnemyController enemy = enemyPool.Get(spawnPosition, target);

            if (enemy != null)
            {
                currentEnemyCount++;

                // Подписаться на смерть врага
                HealthComponent health = enemy.GetComponent<HealthComponent>();
                if (health != null)
                {
                    health.OnDeath += (killer) => OnEnemyDeath(enemy);
                }
            }
        }

        /// <summary>
        /// Обработка смерти врага
        /// </summary>
        private void OnEnemyDeath(EnemyController enemy)
        {
            currentEnemyCount--;

            // Вернуть в пул через 0.5 секунды (время анимации смерти)
            StartCoroutine(ReturnEnemyToPoolDelayed(enemy, 0.5f));
        }

        /// <summary>
        /// Вернуть врага в пул с задержкой
        /// </summary>
        private System.Collections.IEnumerator ReturnEnemyToPoolDelayed(EnemyController enemy, float delay)
        {
            yield return new WaitForSeconds(delay);
            enemyPool.Return(enemy);
        }

        /// <summary>
        /// Запустить/остановить спавн
        /// </summary>
        public void SetSpawning(bool enabled)
        {
            isSpawning = enabled;
        }

        /// <summary>
        /// Очистить всех врагов
        /// </summary>
        public void ClearAllEnemies()
        {
            enemyPool?.ClearPool();
            currentEnemyCount = 0;
        }

        #region Debug
        private void OnGUI()
        {
            if (!Application.isPlaying) return;

            GUI.Label(new Rect(10, 140, 300, 20), 
                $"Active Enemies: {currentEnemyCount} / {maxEnemies}");
        }

        private void OnDrawGizmosSelected()
        {
            if (target == null) return;

            // Визуализация зоны спавна
            Gizmos.color = Color.yellow;
            DrawCircle(target.position, minSpawnDistance);
            
            Gizmos.color = Color.red;
            DrawCircle(target.position, maxSpawnDistance);
        }

        private void DrawCircle(Vector3 center, float radius)
        {
            int segments = 32;
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

                Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0f) * radius;
                Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0f) * radius;

                Gizmos.DrawLine(p1, p2);
            }
        }
        #endregion
    }
}
