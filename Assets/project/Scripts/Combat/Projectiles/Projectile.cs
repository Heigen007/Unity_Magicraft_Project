using UnityEngine;

namespace Magicraft.Combat.Projectiles
{
    /// <summary>
    /// Базовый снаряд
    /// Летит прямо, наносит урон, имеет время жизни и пробитие
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class Projectile : MonoBehaviour
    {
        [Header("Visuals")]
        [Tooltip("Спрайт снаряда")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Effects (Optional)")]
        [Tooltip("Эффект при спавне (опционально)")]
        [SerializeField] private GameObject spawnEffect;

        [Tooltip("Эффект при попадании (опционально)")]
        [SerializeField] private GameObject hitEffect;

        [Tooltip("Эффект при уничтожении (опционально)")]
        [SerializeField] private GameObject destroyEffect;

        // Параметры из CastContext
        private CastContext context;
        private Vector2 direction;
        private float speed;
        private float damage;
        private float lifetime;
        private int remainingPierce;

        // Состояние
        private float aliveTime;
        private bool isInitialized;

        // Для возврата в пул
        private System.Action<Projectile> onReturnToPool;

        private void Awake()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            // Убедимся что спрайт виден
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }
        }

        private void Update()
        {
            if (!isInitialized) return;

            // Движение
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            // Время жизни
            aliveTime += Time.deltaTime;
            if (aliveTime >= lifetime)
            {
                ReturnToPool();
            }

            // Debug - рисуем снаряд в Scene View
            Debug.DrawRay(transform.position, direction * 0.5f, Color.yellow);
        }

        /// <summary>
        /// Инициализировать снаряд с контекстом каста
        /// </summary>
        public void Initialize(CastContext castContext, System.Action<Projectile> returnCallback)
        {
            context = castContext;
            direction = castContext.Direction;
            speed = castContext.ProjectileSpeed;
            damage = castContext.Damage;
            lifetime = castContext.Range / speed; // Дальность через время жизни
            remainingPierce = castContext.Pierce;
            onReturnToPool = returnCallback;

            // Сброс состояния
            aliveTime = 0f;
            isInitialized = true;

            // ПРИНУДИТЕЛЬНО включаем отображение
            gameObject.SetActive(true);
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            // Поворот снаряда в направлении движения
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Эффект спавна
            if (spawnEffect != null)
            {
                Instantiate(spawnEffect, transform.position, Quaternion.identity);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isInitialized) return;

            // НЕ игнорируем триггеры! Враги тоже триггеры!
            // Игнорируем только сам снаряд и его эффекты
            
            // Игнорировать кастера (игрока)
            if (context != null && collision.transform == context.Caster.Muzzle.root)
            {
                return;
            }

            // Проверить, можно ли нанести урон
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                // Нанести урон
                damageable.ApplyDamage(damage, context.Caster.Muzzle.root.gameObject);
                
                Debug.Log($"[Projectile] Hit {collision.name} for {damage} damage!");

                // Эффект попадания
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, Quaternion.identity);
                }

                // Уменьшить пробитие
                remainingPierce--;

                // Если пробитие кончилось - уничтожить
                if (remainingPierce < 0)
                {
                    ReturnToPool();
                    return;
                }
            }
            else
            {
                // Столкновение с непробиваемым объектом (стена и т.д.)
                // Уничтожить снаряд
                ReturnToPool();
            }
        }

        /// <summary>
        /// Вернуть снаряд в пул
        /// </summary>
        private void ReturnToPool()
        {
            if (!isInitialized) return;

            isInitialized = false;

            // Эффект уничтожения
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }

            // Вернуть в пул
            onReturnToPool?.Invoke(this);
        }

        /// <summary>
        /// Принудительно уничтожить снаряд (например, от способности)
        /// </summary>
        public void Destroy()
        {
            ReturnToPool();
        }

        // Debug visualization (можно включить для отладки)
        /*
        private void OnGUI()
        {
            if (!isInitialized) return;

            // Показываем позицию снаряда на экране
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.y = Screen.height - screenPos.y; // Инвертируем Y

            GUI.color = Color.yellow;
            GUI.Label(new Rect(screenPos.x - 20, screenPos.y - 10, 40, 20), "●");
            
            // Инфо о снаряде
            string info = $"V:{speed:F1}";
            GUI.Label(new Rect(screenPos.x + 15, screenPos.y - 5, 100, 20), info);
        }
        */

        private void OnDrawGizmos()
        {
            if (isInitialized && Application.isPlaying)
            {
                // Визуализация направления
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, direction * 0.5f);
            }
        }
    }
}
