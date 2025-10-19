using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.Combat
{
    /// <summary>
    /// Тир/редкость посоха
    /// </summary>
    public enum WandTier
    {
        Common,     // Обычный
        Uncommon,   // Необычный
        Rare,       // Редкий
        Epic,       // Эпический
        Legendary   // Легендарный
    }

    /// <summary>
    /// ScriptableObject для описания посоха
    /// Содержит базовые заклинания, пассивные баффы, тир и скорость атаки
    /// </summary>
    [CreateAssetMenu(fileName = "New Wand", menuName = "Magicraft/Wand", order = 3)]
    public class WandSO : ScriptableObject
    {
        [Header("Идентификация")]
        [Tooltip("Уникальный ID посоха")]
        public string id;

        [Tooltip("Отображаемое имя посоха")]
        public string displayName;

        [Tooltip("Описание посоха")]
        [TextArea(3, 5)]
        public string description;

        [Tooltip("Иконка посоха для UI")]
        public Sprite icon;

        [Tooltip("Тир/редкость посоха")]
        public WandTier tier = WandTier.Common;

        [Header("Заклинания")]
        [Tooltip("Базовые заклинания посоха (в порядке каста)")]
        [SerializeField]
        public List<SpellSO> baseSpells = new List<SpellSO>();

        [Tooltip("Максимальное количество заклинаний в посохе")]
        [Range(1, 10)]
        public int maxSpellSlots = 3;

        [Header("Пассивные эффекты")]
        [Tooltip("Постоянные баффы от посоха")]
        [SerializeField]
        public List<BuffSO> passiveBuffs = new List<BuffSO>();

        [Header("Параметры каста")]
        [Tooltip("Скорость атаки (кастов в секунду)")]
        [Range(0.1f, 10f)]
        public float attackSpeed = 1f;

        [Tooltip("Задержка между кастами (секунды)")]
        [Min(0)]
        public float castDelay = 0.5f;

        [Tooltip("Время перезарядки всех заклинаний (секунды, 0 = нет перезарядки)")]
        [Min(0)]
        public float rechargeTime = 0f;

        [Header("Модификаторы")]
        [Tooltip("Общий множитель урона посоха")]
        [Range(0.1f, 5f)]
        public float damageMultiplier = 1f;

        [Tooltip("Общий множитель стоимости маны")]
        [Range(0.1f, 5f)]
        public float manaCostMultiplier = 1f;

        [Tooltip("Общий множитель кулдауна")]
        [Range(0.1f, 5f)]
        public float cooldownMultiplier = 1f;

        [Header("Визуал")]
        [Tooltip("Спрайт посоха (для игрока)")]
        public Sprite wandSprite;

        [Tooltip("Цвет частиц посоха")]
        public Color particleColor = Color.white;

        [Tooltip("Префаб визуального эффекта каста")]
        public GameObject castEffectPrefab;

        /// <summary>
        /// Проверка валидности WandSO
        /// </summary>
        private void OnValidate()
        {
            // Автоматическая генерация ID
            if (string.IsNullOrEmpty(id))
            {
                id = name.ToLower().Replace(" ", "_");
            }

            // Автоматическая генерация имени
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = name;
            }

            // Валидация заклинаний
            if (baseSpells.Count > maxSpellSlots)
            {
                Debug.LogWarning($"[WandSO] '{displayName}': Количество заклинаний ({baseSpells.Count}) превышает maxSpellSlots ({maxSpellSlots})!");
            }

            // Удаление null-ссылок из списков
            baseSpells.RemoveAll(spell => spell == null);
            passiveBuffs.RemoveAll(buff => buff == null);
        }

        /// <summary>
        /// Получить количество заклинаний
        /// </summary>
        public int GetSpellCount()
        {
            return baseSpells.Count;
        }

        /// <summary>
        /// Получить заклинание по индексу
        /// </summary>
        public SpellSO GetSpell(int index)
        {
            if (index >= 0 && index < baseSpells.Count)
            {
                return baseSpells[index];
            }
            return null;
        }

        /// <summary>
        /// Добавить заклинание (если есть место)
        /// </summary>
        public bool TryAddSpell(SpellSO spell)
        {
            if (spell == null) return false;
            if (baseSpells.Count >= maxSpellSlots) return false;

            baseSpells.Add(spell);
            return true;
        }

        /// <summary>
        /// Удалить заклинание по индексу
        /// </summary>
        public bool TryRemoveSpell(int index)
        {
            if (index >= 0 && index < baseSpells.Count)
            {
                baseSpells.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Получить все пассивные баффы
        /// </summary>
        public List<BuffSO> GetPassiveBuffs()
        {
            return new List<BuffSO>(passiveBuffs);
        }

        /// <summary>
        /// Получить задержку между кастами с учётом скорости атаки
        /// </summary>
        public float GetCastDelay()
        {
            return castDelay / attackSpeed;
        }

        /// <summary>
        /// Получить цвет тира для UI
        /// </summary>
        public Color GetTierColor()
        {
            switch (tier)
            {
                case WandTier.Common:
                    return new Color(0.7f, 0.7f, 0.7f); // Серый
                case WandTier.Uncommon:
                    return new Color(0.2f, 0.8f, 0.2f); // Зелёный
                case WandTier.Rare:
                    return new Color(0.2f, 0.5f, 1f);   // Синий
                case WandTier.Epic:
                    return new Color(0.8f, 0.2f, 0.8f); // Фиолетовый
                case WandTier.Legendary:
                    return new Color(1f, 0.6f, 0.2f);   // Оранжевый
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Получить описание посоха для UI
        /// </summary>
        public string GetFullDescription()
        {
            string desc = description;

            desc += $"\n\nТир: {tier}";
            desc += $"\nСкорость атаки: {attackSpeed:F1}";
            desc += $"\nЗаклинаний: {baseSpells.Count}/{maxSpellSlots}";

            if (passiveBuffs.Count > 0)
            {
                desc += $"\n\nПассивные эффекты:";
                foreach (var buff in passiveBuffs)
                {
                    if (buff != null)
                    {
                        desc += $"\n• {buff.DisplayName}";
                    }
                }
            }

            return desc;
        }
    }
}
