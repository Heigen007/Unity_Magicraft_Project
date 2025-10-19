using UnityEngine;
using System;

namespace Magicraft.Player
{
    /// <summary>
    /// Компонент маны для игрока
    /// Управляет текущей маной, максимумом и регенерацией
    /// </summary>
    public class ManaComponent : MonoBehaviour
    {
        [Header("Mana Settings")]
        [Tooltip("Максимальное количество маны")]
        [SerializeField] private float maxMana = 100f;

        [Tooltip("Начальная мана при старте (если 0, то заполняется до максимума)")]
        [SerializeField] private float startingMana = 0f;

        [Tooltip("Регенерация маны в секунду")]
        [SerializeField] private float manaRegenPerSecond = 10f;

        [Tooltip("Задержка перед началом регенерации после траты маны (секунды)")]
        [SerializeField] private float regenDelay = 0.5f;

        // Текущее состояние
        private float currentMana;
        private float timeSinceLastSpend;

        // Свойства
        public float CurrentMana => currentMana;
        public float MaxMana => maxMana;
        public float ManaPercent => maxMana > 0f ? currentMana / maxMana : 0f;
        public bool IsFull => currentMana >= maxMana;
        public bool IsEmpty => currentMana <= 0f;

        // События
        public event Action<float, float> OnManaChanged; // (current, max)
        public event Action OnManaEmpty;
        public event Action OnManaFull;
        public event Action<float> OnManaSpent; // (amount)

        private void Awake()
        {
            // Установить начальную ману
            if (startingMana <= 0f)
            {
                currentMana = maxMana;
            }
            else
            {
                currentMana = Mathf.Min(startingMana, maxMana);
            }

            timeSinceLastSpend = regenDelay; // Начинаем с возможностью регенерации
        }

        private void Start()
        {
            // Уведомить UI о начальном значении
            OnManaChanged?.Invoke(currentMana, maxMana);
        }

        private void Update()
        {
            RegenerateMana(Time.deltaTime);
        }

        /// <summary>
        /// Попытаться потратить ману
        /// </summary>
        /// <param name="amount">Количество маны для траты</param>
        /// <returns>true если мана была успешно потрачена</returns>
        public bool TrySpend(float amount)
        {
            if (amount < 0f)
            {
                Debug.LogWarning("[ManaComponent] Попытка потратить отрицательное количество маны!");
                return false;
            }

            if (currentMana < amount)
            {
                // Недостаточно маны
                OnManaEmpty?.Invoke();
                return false;
            }

            // Потратить ману
            currentMana -= amount;
            currentMana = Mathf.Max(0f, currentMana);
            timeSinceLastSpend = 0f; // Сбросить таймер регенерации

            OnManaSpent?.Invoke(amount);
            OnManaChanged?.Invoke(currentMana, maxMana);

            if (currentMana <= 0f)
            {
                OnManaEmpty?.Invoke();
            }

            return true;
        }

        /// <summary>
        /// Добавить ману (например, от зелья)
        /// </summary>
        /// <param name="amount">Количество маны для добавления</param>
        public void Add(float amount)
        {
            if (amount <= 0f) return;

            bool wasFull = IsFull;

            currentMana += amount;
            currentMana = Mathf.Min(currentMana, maxMana);

            OnManaChanged?.Invoke(currentMana, maxMana);

            if (!wasFull && IsFull)
            {
                OnManaFull?.Invoke();
            }
        }

        /// <summary>
        /// Установить текущую ману
        /// </summary>
        /// <param name="amount">Новое значение маны</param>
        public void SetMana(float amount)
        {
            currentMana = Mathf.Clamp(amount, 0f, maxMana);
            OnManaChanged?.Invoke(currentMana, maxMana);
        }

        /// <summary>
        /// Полностью восстановить ману
        /// </summary>
        public void FillToMax()
        {
            SetMana(maxMana);
            OnManaFull?.Invoke();
        }

        /// <summary>
        /// Установить максимальную ману (например, при апгрейде)
        /// </summary>
        /// <param name="newMax">Новый максимум маны</param>
        /// <param name="fillToNew">Заполнить ли до нового максимума</param>
        public void SetMaxMana(float newMax, bool fillToNew = false)
        {
            maxMana = Mathf.Max(0f, newMax);

            if (fillToNew)
            {
                currentMana = maxMana;
            }
            else
            {
                currentMana = Mathf.Min(currentMana, maxMana);
            }

            OnManaChanged?.Invoke(currentMana, maxMana);
        }

        /// <summary>
        /// Изменить скорость регенерации (например, от бафа)
        /// </summary>
        public void SetRegenRate(float newRegenPerSecond)
        {
            manaRegenPerSecond = Mathf.Max(0f, newRegenPerSecond);
        }

        /// <summary>
        /// Регенерация маны со временем
        /// </summary>
        private void RegenerateMana(float deltaTime)
        {
            if (IsFull) return;

            // Обновить таймер задержки
            timeSinceLastSpend += deltaTime;

            // Если задержка ещё не прошла, не регенерировать
            if (timeSinceLastSpend < regenDelay) return;

            // Регенерировать ману
            bool wasFull = IsFull;

            currentMana += manaRegenPerSecond * deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);

            OnManaChanged?.Invoke(currentMana, maxMana);

            if (!wasFull && IsFull)
            {
                OnManaFull?.Invoke();
            }
        }

        /// <summary>
        /// Проверить, достаточно ли маны
        /// </summary>
        public bool HasEnough(float amount)
        {
            return currentMana >= amount;
        }

        #region Debug
        private void OnValidate()
        {
            maxMana = Mathf.Max(1f, maxMana);
            manaRegenPerSecond = Mathf.Max(0f, manaRegenPerSecond);
            regenDelay = Mathf.Max(0f, regenDelay);

            if (Application.isPlaying)
            {
                currentMana = Mathf.Min(currentMana, maxMana);
            }
        }

        // Для дебага в инспекторе
        [ContextMenu("Fill Mana")]
        private void DebugFillMana()
        {
            FillToMax();
            Debug.Log("[ManaComponent] Мана заполнена до максимума");
        }

        [ContextMenu("Empty Mana")]
        private void DebugEmptyMana()
        {
            SetMana(0f);
            Debug.Log("[ManaComponent] Мана опустошена");
        }

        [ContextMenu("Spend 20 Mana")]
        private void DebugSpendMana()
        {
            bool success = TrySpend(20f);
            Debug.Log($"[ManaComponent] Попытка потратить 20 маны: {(success ? "успешно" : "недостаточно маны")}");
        }
        #endregion
    }
}
