using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [Tooltip("Неуязвимость после получения урона (в секундах) - ОТКЛЮЧЕНА ДЛЯ НЕСКОЛЬКИХ ВРАГОВ")]
        [SerializeField] private float invulnerabilityDuration = 0f;

        [Header("Player Auto-Revive")]
        [Tooltip("Автоматическое воскрешение для игрока (только если тег = Player)")]
        [SerializeField] private bool autoRevivePlayer = true;

        // Текущее состояние
        private float currentHealth;
        private float timeSinceLastDamage;
        
        // Таймеры неуязвимости ДЛЯ КАЖДОГО ВРАГА ОТДЕЛЬНО (не общий!)
        private Dictionary<GameObject, float> invulnerabilityTimers = new Dictionary<GameObject, float>();
        
        private bool isPlayer;

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
            // Проверка, является ли это игроком
            isPlayer = CompareTag("Player");

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
            
            // Обновление таймеров неуязвимости ДЛЯ КАЖДОГО врага
            // Используем ToList() чтобы избежать "Collection was modified" исключения
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var attacker in invulnerabilityTimers.Keys.ToList())
            {
                float timer = invulnerabilityTimers[attacker];
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    toRemove.Add(attacker);
                }
                else
                {
                    invulnerabilityTimers[attacker] = timer;
                }
            }
            foreach (var attacker in toRemove)
            {
                invulnerabilityTimers.Remove(attacker);
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

            // Проверка неуязвимости ДЛЯ ЭТОГО КОНКРЕТНОГО ВРАГА
            // (каждый враг имеет свой таймер неуязвимости, враги могут атаковать одновременно!)
            if (attacker != null && invulnerabilityTimers.ContainsKey(attacker) && invulnerabilityTimers[attacker] > 0f)
            {
                // Этот враг только что атаковал, пропускаем
                return;
            }

            // Применить урон
            float oldHealth = currentHealth;
            currentHealth = Mathf.Max(0f, currentHealth - damage);

            // Сбросить таймер регенерации
            timeSinceLastDamage = 0f;

            // Запустить неуязвимость ТОЛЬКО ДЛЯ ЭТОГО ВРАГА
            if (invulnerabilityDuration > 0f && attacker != null)
            {
                invulnerabilityTimers[attacker] = invulnerabilityDuration;
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

            // Автоматическое воскрешение игрока
            if (isPlayer && autoRevivePlayer)
            {
                Debug.Log($"[HealthComponent] Player died! Auto-reviving with full health (100 HP)...");
                Revive(1f); // Воскресить со 100% здоровья
            }
        }

        /// <summary>
        /// Воскресить
        /// </summary>
        public void Revive(float healthPercent = 1f)
        {
            if (IsAlive) return;

            currentHealth = maxHealth * Mathf.Clamp01(healthPercent);
            timeSinceLastDamage = regenDelay;
            
            // Очистить таймеры неуязвимости при воскрешении
            invulnerabilityTimers.Clear();

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

            // Отображение HP над объектом - БОЛЬШОЙ ТЕКСТ
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            if (screenPos.z > 0)
            {
                screenPos.y = Screen.height - screenPos.y;
                
                // УВЕЛИЧЕННЫЙ размер и жирный шрифт
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontSize = 24; // Был 12, теперь 24
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = isPlayer ? Color.green : Color.red;
                
                // Черная обводка для читаемости
                GUI.color = Color.black;
                GUI.Label(new Rect(screenPos.x - 49, screenPos.y - 1, 100, 30), $"{currentHealth:F0}/{maxHealth:F0}", style);
                GUI.Label(new Rect(screenPos.x - 51, screenPos.y - 1, 100, 30), $"{currentHealth:F0}/{maxHealth:F0}", style);
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y - 2, 100, 30), $"{currentHealth:F0}/{maxHealth:F0}", style);
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y, 100, 30), $"{currentHealth:F0}/{maxHealth:F0}", style);
                
                // Основной текст
                GUI.color = Color.white;
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y - 1, 100, 30), $"{currentHealth:F0}/{maxHealth:F0}", style);
            }
        }
        #endregion
        // */
    }
}
