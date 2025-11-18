using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Контроллер врага: простое AI - движение к игроку
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyController : MonoBehaviour
    {
        // Static событие для RewardSystem
        public static event System.Action<GameObject> OnAnyEnemyKilled;

        [Header("Movement")]
        [Tooltip("Скорость движения")]
        [SerializeField] private float moveSpeed = 3f;

        [Header("References")]
        [Tooltip("Цель (обычно игрок, если null - ищется автоматически)")]
        [SerializeField] private Transform target;

        [Header("Visuals")]
        [Tooltip("SpriteRenderer для визуальных эффектов")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Компоненты
        private HealthComponent healthComponent;
        private Rigidbody2D rb;

        // Состояние
        private bool isActive = true;

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            rb = GetComponent<Rigidbody2D>();

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            // Настройка Rigidbody2D для врага
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                // НЕ меняем bodyType - оставляем Dynamic для корректной работы триггеров
            }

            // Подписка на смерть
            if (healthComponent != null)
            {
                healthComponent.OnDeath += OnDeath;
            }
        }

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
        }

        private void FixedUpdate()
        {
            if (!isActive) return;
            if (!healthComponent.IsAlive) return;
            if (target == null) return;

            // Движение к цели
            MoveToTarget();
        }

        /// <summary>
        /// Движение к цели
        /// </summary>
        private void MoveToTarget()
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, target.position);

            // Двигаться всегда в направлении цели (убрал stopDistance - враги не останавливаются)
            // Используем MovePosition для движения без физических столкновений
            Vector2 movement = direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }

        /// <summary>
        /// Обработка смерти
        /// </summary>
        private void OnDeath(GameObject killer)
        {
            isActive = false;

            // Вызвать static событие для систем (RewardSystem и т.д.)
            OnAnyEnemyKilled?.Invoke(killer);

            // Сразу вернуть в пул (без задержки)
            ReturnToPool();
        }

        /// <summary>
        /// Вернуть врага в пул
        /// </summary>
        private void ReturnToPool()
        {
            // Будет реализовано через EnemyPool
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Инициализация врага при получении из пула
        /// </summary>
        public void Initialize(Transform newTarget)
        {
            target = newTarget;
            isActive = true;

            // Сброс здоровья
            if (healthComponent != null && !healthComponent.IsAlive)
            {
                healthComponent.Revive(1f);
            }

            // Сброс визуала
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// Установить цель
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.OnDeath -= OnDeath;
            }
        }
    }
}
