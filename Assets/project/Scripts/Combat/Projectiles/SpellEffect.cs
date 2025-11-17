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

        [Header("References")]
        [Tooltip("Animator для анимации")]
        [SerializeField] private Animator animator;

        [Tooltip("Collider для урона")]
        [SerializeField] private Collider2D damageCollider;

        // Параметры из CastContext
        private CastContext context;
        private float aliveTime;
        private bool isInitialized;
        private bool hasDealtDamage;

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
            hasDealtDamage = false;

            gameObject.SetActive(true);

            // Воспроизвести анимацию
            if (animator != null)
            {
                animator.Play(0, 0, 0f); // Играть с начала
            }

            // Настроить размер коллайдера
            if (damageCollider != null && damageCollider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = damageRadius;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInitialized) return;
            if (hasDealtDamage) return; // Урон только один раз

            // Игнорировать триггеры
            if (collision.isTrigger) return;

            // Игнорировать кастера
            if (context != null && collision.transform == context.Caster.Muzzle.root)
            {
                return;
            }

            // Проверить, можно ли нанести урон
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.ApplyDamage(context.Damage, context.Caster.Muzzle.root.gameObject);
                hasDealtDamage = true; // Урон нанесён
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
