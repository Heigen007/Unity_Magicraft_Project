using UnityEngine;
using Magicraft.Util;

namespace Magicraft.Combat.Enemy
{
    /// <summary>
    /// Пул для врагов
    /// Управляет созданием и переиспользованием врагов
    /// </summary>
    public class EnemyPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [Tooltip("Массив префабов врагов (разные типы)")]
        public EnemyController[] enemyPrefabs;

        [Tooltip("Начальный размер пула для каждого типа")]
        public int initialPoolSizePerType = 15;

        [Tooltip("Родитель для врагов в иерархии")]
        [SerializeField] private Transform poolParent;

        // Массив пулов (по одному на тип врага)
        private ObjectPool<EnemyController>[] pools;

        // Singleton
        private static EnemyPool instance;
        public static EnemyPool Instance => instance;

        /// <summary>
        /// Публичный метод инициализации
        /// </summary>
        public void Initialize()
        {
            if (pools != null)
            {
                return; // Уже инициализирован
            }

            InitializePool();
        }

        private void Awake()
        {
            // Singleton
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            InitializePool();
        }

        /// <summary>
        /// Инициализация пула
        /// </summary>
        private void InitializePool()
        {
            // Создать родителя для пула, если не задан
            if (poolParent == null)
            {
                GameObject poolObj = new GameObject("Enemy Pool");
                poolParent = poolObj.transform;
                poolParent.SetParent(transform);
            }

            // Проверить массив префабов
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                Debug.LogError("[EnemyPool] Enemy Prefabs не назначены!", this);
                return;
            }

            // Создать пул для каждого типа врага
            pools = new ObjectPool<EnemyController>[enemyPrefabs.Length];
            
            for (int i = 0; i < enemyPrefabs.Length; i++)
            {
                if (enemyPrefabs[i] != null)
                {
                    pools[i] = new ObjectPool<EnemyController>(enemyPrefabs[i], poolParent, initialPoolSizePerType);
                }
                else
                {
                    Debug.LogError($"[EnemyPool] Enemy Prefab [{i}] не назначен!", this);
                }
            }
        }

        /// <summary>
        /// Получить врага из пула (случайный тип)
        /// </summary>
        public EnemyController Get(Vector3 position, Transform target)
        {
            if (pools == null || pools.Length == 0)
            {
                Debug.LogError("[EnemyPool] Пулы не инициализированы!");
                return null;
            }

            // Выбрать случайный тип врага
            int randomIndex = Random.Range(0, pools.Length);
            return GetByType(randomIndex, position, target);
        }

        /// <summary>
        /// Получить врага конкретного типа из пула
        /// </summary>
        public EnemyController GetByType(int typeIndex, Vector3 position, Transform target)
        {
            if (pools == null || typeIndex < 0 || typeIndex >= pools.Length)
            {
                Debug.LogError($"[EnemyPool] Неверный индекс типа: {typeIndex}");
                return null;
            }

            if (pools[typeIndex] == null)
            {
                Debug.LogError($"[EnemyPool] Пул типа {typeIndex} не создан!");
                return null;
            }

            EnemyController enemy = pools[typeIndex].Get();
            
            if (enemy == null)
            {
                Debug.LogError($"[EnemyPool] pool[{typeIndex}].Get() вернул null!");
                return null;
            }

            enemy.transform.position = position;
            enemy.Initialize(target);

            return enemy;
        }

        /// <summary>
        /// Вернуть врага в пул
        /// </summary>
        public void Return(EnemyController enemy)
        {
            if (pools == null || enemy == null) return;

            // Найти правильный пул для этого врага
            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i] != null && pools[i].BelongsToPool(enemy))
                {
                    pools[i].Return(enemy);
                    return;
                }
            }
        }

        /// <summary>
        /// Очистить все пулы
        /// </summary>
        public void ClearPool()
        {
            if (pools != null)
            {
                foreach (var pool in pools)
                {
                    pool?.Clear();
                }
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        #region Debug
        private void OnGUI()
        {
            if (!Application.isPlaying) return;

            // Отображение статистики всех пулов
            if (pools != null)
            {
                for (int i = 0; i < pools.Length; i++)
                {
                    if (pools[i] != null)
                    {
                        GUI.Label(new Rect(10, 120 + i * 20, 300, 20), 
                            $"Enemy Type {i + 1}: {pools[i].AvailableCount} available / {pools[i].TotalCount} total");
                    }
                }
            }
        }
        #endregion
    }
}
