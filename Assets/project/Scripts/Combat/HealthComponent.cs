using UnityEngine;
using System;

namespace Magicraft.Combat
{
    /// <summary>
    /// Компонент здоровья для игрока и врагов
    /// Управляет HP, уроном, смертью, регенерацией
    /// Реализует IDamageable
    /// </summary>
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [Header("Health Settings")]
        [Tooltip("Максимальное здоровье")]
        [SerializeField] private float maxHealth = 100f;

        [Tooltip("Начальное здоровье (если 0, то = maxHealth)")]
        [SerializeField] private float startHealth = 0f;

        [Header("Regeneration")]
        [Tooltip("Регенерация HP в секунду")]
        [SerializeField] private float healthRegen = 0f;

        [Tooltip("Задержка перед началом регенерации после урона")]
        [SerializeField] private float regenDelay = 3f;

        [Header("Invulnerability")]
        [Tooltip("Неуязвимость после получения урона (в секундах)")]
        [SerializeField] private float invulnerabilityDuration = 0f;

        // Текущее состояние
        private float currentHealth;
        private float timeSinceLastDamage;
        private float invulnerabilityTimer;

        // События
        public event Action<float, float> OnHealthChanged; // (current, max)
        public event Action<float, GameObject> OnDamaged; // (damage, attacker)
        public event Action<GameObject> OnDeath; // (killer)
        public event Action OnRevived;

        // Свойства для IDamageable
        public bool IsAlive => currentHealth > 0f;
        public Transform Transform => transform;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercent => maxHealth > 0 ? currentHealth / maxHealth : 0f;

        private void Awake()
        {
            // Инициализация здоровья
            if (startHealth <= 0f)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = Mathf.Min(startHealth, maxHealth);
            }

            timeSinceLastDamage = regenDelay; // Начинаем с возможностью регена
        }

        private void Update()
        {
            if (!IsAlive) return;

            // Обновление таймеров
            timeSinceLastDamage += Time.deltaTime;
            
            if (invulnerabilityTimer > 0f)
            {
                invulnerabilityTimer -= Time.deltaTime;
            }

            // Регенерация
            if (healthRegen > 0f && timeSinceLastDamage >= regenDelay)
            {
                Heal(healthRegen * Time.deltaTime);
            }
        }

        /// <summary>
        /// Применить урон
        /// </summary>
        public void ApplyDamage(float damage, GameObject attacker)
        {
            if (!IsAlive) return;
            if (damage <= 0f) return;

            // Проверка неуязвимости
            if (invulnerabilityTimer > 0f)
            {
                return;
            }

            // Применить урон
            float oldHealth = currentHealth;
            currentHealth = Mathf.Max(0f, currentHealth - damage);

            // Сбросить таймер регенерации
            timeSinceLastDamage = 0f;

            // Запустить неуязвимость
            if (invulnerabilityDuration > 0f)
            {
                invulnerabilityTimer = invulnerabilityDuration;
            }

            // События
            OnDamaged?.Invoke(damage, attacker);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Проверка смерти
            if (currentHealth <= 0f && oldHealth > 0f)
            {
                Die(attacker);
            }
        }

        /// <summary>
        /// Исцелить
        /// </summary>
        public void Heal(float amount)
        {
            if (!IsAlive) return;
            if (amount <= 0f) return;

            float oldHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

            if (currentHealth != oldHealth)
            {
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }

        /// <summary>
        /// Установить максимальное здоровье
        /// </summary>
        public void SetMaxHealth(float newMax, bool healToFull = false)
        {
            maxHealth = Mathf.Max(1f, newMax);

            if (healToFull)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth = Mathf.Min(currentHealth, maxHealth);
            }

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Смерть
        /// </summary>
        private void Die(GameObject killer)
        {
            currentHealth = 0f;
            OnDeath?.Invoke(killer);
        }

        /// <summary>
        /// Воскресить
        /// </summary>
        public void Revive(float healthPercent = 1f)
        {
            if (IsAlive) return;

            currentHealth = maxHealth * Mathf.Clamp01(healthPercent);
            timeSinceLastDamage = regenDelay;
            invulnerabilityTimer = 0f;

            OnRevived?.Invoke();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Мгновенная смерть (для дебага)
        /// </summary>
        public void Kill()
        {
            if (!IsAlive) return;
            ApplyDamage(currentHealth, null);
        }

        // Debug отключен - используй UI для отображения HP
        // /*
        #region Debug
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            if (!IsAlive) return;

            // Отображение HP над объектом
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            if (screenPos.z > 0)
            {
                screenPos.y = Screen.height - screenPos.y;
                GUI.color = Color.red;
                GUI.Label(new Rect(screenPos.x - 30, screenPos.y, 60, 20), 
                    $"{currentHealth:F0}/{maxHealth:F0}");
            }
        }
        #endregion
        // */
    }
}
