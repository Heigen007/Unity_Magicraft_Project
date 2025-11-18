using UnityEngine;

namespace Magicraft.Combat.Projectiles
{
    /// <summary>
    /// Эффект заклинания, который появляется в точке и воспроизводит анимацию
    /// Используется для AoE заклинаний (grenade, atomic bomb, water splash)
    /// </summary>
    public class SpellEffect : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Время жизни эффекта (секунды)")]
        [SerializeField] private float lifetime = 1f;

        [Tooltip("Радиус урона")]
        [SerializeField] private float damageRadius = 1f;

        [Header("Damage")]
        [Tooltip("Слой врагов для поиска")]
        [SerializeField] private LayerMask enemyLayer;

        [Header("References")]
        [Tooltip("Animator для анимации")]
        [SerializeField] private Animator animator;

        [Tooltip("Collider для урона")]
        [SerializeField] private Collider2D damageCollider;

        [Header("Debug")]
        [Tooltip("Показывать зону урона в Scene View")]
        [SerializeField] private bool showDamageRadius = true;

        [Tooltip("Показывать отладочные сообщения")]
        [SerializeField] private bool debugMessages = true;

        // Параметры из CastContext
        private CastContext context;
        private float aliveTime;
        private bool isInitialized;

        // Для возврата в пул
        private System.Action<SpellEffect> onReturnToPool;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider2D>();
            }
        }

        private void Update()
        {
            if (!isInitialized) return;

            aliveTime += Time.deltaTime;

            // Наносим урон каждый кадр (покуда анимация активна)
            DealAoEDamage();

            if (aliveTime >= lifetime)
            {
                ReturnToPool();
            }
        }

        /// <summary>
        /// Инициализировать эффект
        /// </summary>
        public void Initialize(CastContext castContext, System.Action<SpellEffect> returnCallback)
        {
            context = castContext;
            onReturnToPool = returnCallback;

            aliveTime = 0f;
            isInitialized = true;

            gameObject.SetActive(true);

            // Воспроизвести анимацию
            if (animator != null)
            {
                animator.Play(0, 0, 0f); // Играть с начала
            }

            // Настроить размер коллайдера
            if (damageCollider != null)
            {
                if (damageCollider is CircleCollider2D circleCollider)
                {
                    circleCollider.radius = damageRadius;
                }
                else if (damageCollider is BoxCollider2D boxCollider)
                {
                    boxCollider.size = Vector2.one * damageRadius * 2f; // Box размер = радиус * 2
                }
                
                if (debugMessages)
                {
                    Debug.Log($"[SpellEffect] Damage radius set to: {damageRadius}");
                }
            }

            // СРАЗУ наносим урон ВСЕМ врагам в радиусе
            DealAoEDamage();
        }

        /// <summary>
        /// Наносит урон ВСЕМ врагам в радиусе (каждый кадр во время анимации)
        /// Каждый враг отслеживает собственный таймер неуязвимости через HealthComponent
        /// </summary>
        private void DealAoEDamage()
        {

            // Найти ВСЕ коллайдеры в радиусе
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRadius, enemyLayer);

            if (debugMessages)
            {
                Debug.Log($"[SpellEffect] Found {colliders.Length} colliders in radius {damageRadius}");
            }

            // Наносим урон каждому врагу
            foreach (var collider in colliders)
            {
                // Игнорировать кастера (игрока)
                if (context != null && collider.transform == context.Caster.Muzzle.root)
                {
                    continue;
                }

                // Проверить, можно ли нанести урон
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    if (debugMessages)
                    {
                        Debug.Log($"[SpellEffect] Dealing {context.Damage} damage to {collider.name}");
                    }
                    
                    damageable.ApplyDamage(context.Damage, context.Caster.Muzzle.root.gameObject);
                }
                else if (debugMessages)
                {
                    Debug.Log($"[SpellEffect] {collider.name} has no IDamageable or is dead");
                }
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Урон наносится при инициализации через DealAoEDamage(), а не здесь
            // Этот метод оставляем для совместимости, но он больше не используется
            if (debugMessages)
            {
                Debug.Log($"[SpellEffect] OnTriggerEnter2D called (deprecated, use DealAoEDamage)");
            }
        }

        private void OnDrawGizmos()
        {
            // Всегда показываем границы, если объект активен
            if (isInitialized && showDamageRadius)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // Красная полупрозрачная - видна всегда
                DrawWireCircle(transform.position, damageRadius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Когда выбран объект - показываем более яркую обводку
            if (showDamageRadius)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1f); // Ярко красная
                DrawWireCircle(transform.position, damageRadius);
            }
        }

        /// <summary>
        /// Рисует окружность для визуализации зоны урона
        /// </summary>
        private void DrawWireCircle(Vector3 center, float radius)
        {
            const int segments = 32;
            float angleStep = 360f / segments;
            Vector3 lastPoint = center + new Vector3(radius, 0f, 0f);

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }

        /// <summary>
        /// Вернуть эффект в пул
        /// </summary>
        private void ReturnToPool()
        {
            isInitialized = false;
            onReturnToPool?.Invoke(this);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Установить время жизни
        /// </summary>
        public void SetLifetime(float time)
        {
            lifetime = time;
        }

        /// <summary>
        /// Установить радиус урона
        /// </summary>
        public void SetDamageRadius(float radius)
        {
            damageRadius = radius;
        }
    }
}
