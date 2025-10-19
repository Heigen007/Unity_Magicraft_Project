using UnityEngine;
using Magicraft.Util;

namespace Magicraft.Combat.Projectiles
{
    /// <summary>
    /// Пул для снарядов
    /// Управляет созданием и переиспользованием снарядов
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [Tooltip("Префаб снаряда")]
        public Projectile projectilePrefab; // ПУБЛИЧНОЕ поле для Inspector

        [Tooltip("Начальный размер пула")]
        public int initialPoolSize = 20; // ПУБЛИЧНОЕ поле для Inspector

        [Tooltip("Родитель для снарядов в иерархии")]
        [SerializeField] private Transform poolParent;

        // Пул
        private ObjectPool<Projectile> pool;

        // Singleton
        private static ProjectilePool instance;
        public static ProjectilePool Instance => instance;

        /// <summary>
        /// Публичный метод инициализации (вызывается извне если Awake не сработал)
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
                GameObject poolObj = new GameObject("Projectile Pool");
                poolParent = poolObj.transform;
                poolParent.SetParent(transform);
            }

            // Создать пул
            if (projectilePrefab != null)
            {
                pool = new ObjectPool<Projectile>(projectilePrefab, poolParent, initialPoolSize);
            }
            else
            {
                Debug.LogError("[ProjectilePool] Projectile Prefab не назначен!", this);
            }
        }

        /// <summary>
        /// Получить снаряд из пула и инициализировать
        /// </summary>
        public Projectile Get(CastContext context)
        {
            if (pool == null)
            {
                Debug.LogError("[ProjectilePool] Пул не инициализирован!");
                return null;
            }

            Projectile projectile = pool.Get();
            
            if (projectile == null)
            {
                Debug.LogError("[ProjectilePool] pool.Get() вернул null!");
                return null;
            }
            
            projectile.transform.position = context.SpawnPosition;
            projectile.Initialize(context, ReturnProjectile);

            return projectile;
        }

        /// <summary>
        /// Вернуть снаряд в пул
        /// </summary>
        private void ReturnProjectile(Projectile projectile)
        {
            if (pool != null && projectile != null)
            {
                pool.Return(projectile);
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
                GUI.Label(new Rect(10, 100, 300, 20), 
                    $"Projectile Pool: {pool.AvailableCount} available / {pool.TotalCount} total");
            }
        }
        #endregion
    }
}
