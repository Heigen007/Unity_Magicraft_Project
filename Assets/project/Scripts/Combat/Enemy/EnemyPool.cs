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
        [Tooltip("Префаб врага")]
        public EnemyController enemyPrefab;

        [Tooltip("Начальный размер пула")]
        public int initialPoolSize = 30;

        [Tooltip("Родитель для врагов в иерархии")]
        [SerializeField] private Transform poolParent;

        // Пул
        private ObjectPool<EnemyController> pool;

        // Singleton
        private static EnemyPool instance;
        public static EnemyPool Instance => instance;

        /// <summary>
        /// Публичный метод инициализации
        /// </summary>
        public void Initialize()
        {
            if (pool != null)
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

            // Создать пул
            if (enemyPrefab != null)
            {
                pool = new ObjectPool<EnemyController>(enemyPrefab, poolParent, initialPoolSize);
            }
            else
            {
                Debug.LogError("[EnemyPool] Enemy Prefab не назначен!", this);
            }
        }

        /// <summary>
        /// Получить врага из пула
        /// </summary>
        public EnemyController Get(Vector3 position, Transform target)
        {
            if (pool == null)
            {
                Debug.LogError("[EnemyPool] Пул не инициализирован!");
                return null;
            }

            EnemyController enemy = pool.Get();
            
            if (enemy == null)
            {
                Debug.LogError("[EnemyPool] pool.Get() вернул null!");
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
            if (pool != null && enemy != null)
            {
                pool.Return(enemy);
            }
        }

        /// <summary>
        /// Очистить весь пул
        /// </summary>
        public void ClearPool()
        {
            pool?.Clear();
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

            // Отображение статистики пула
            if (pool != null)
            {
                GUI.Label(new Rect(10, 120, 300, 20), 
                    $"Enemy Pool: {pool.AvailableCount} available / {pool.TotalCount} total");
            }
        }
        #endregion
    }
}
