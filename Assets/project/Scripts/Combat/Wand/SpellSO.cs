using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.Combat
{
    /// <summary>
    /// Типы исполнения заклинания
    /// </summary>
    public enum SpellExecutionType
    {
        Projectile,  // Снаряд
        Beam,        // Луч
        AoE,         // Область поражения
        Self         // Баф на себя
    }

    /// <summary>
    /// Теги для категоризации заклинаний (для фильтрации бафов)
    /// </summary>
    public enum SpellTag
    {
        None,
        Fire,
        Ice,
        Lightning,
        Arcane,
        Physical,
        Projectile,
        AoE,
        Beam
    }

    /// <summary>
    /// ScriptableObject для описания заклинания
    /// Определяет базовые параметры заклинания (без модификаторов)
    /// </summary>
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magicraft/Spell", order = 1)]
    public class SpellSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Уникальный идентификатор заклинания")]
        public string Id;
        
        [Tooltip("Отображаемое название")]
        public string DisplayName;
        
        [Tooltip("Иконка для UI")]
        public Sprite Icon;
        
        [TextArea(3, 5)]
        [Tooltip("Описание заклинания")]
        public string Description;

        [Header("Execution")]
        [Tooltip("Тип исполнения заклинания")]
        public SpellExecutionType ExecutionType = SpellExecutionType.Projectile;

        [Tooltip("Префаб снаряда (для Projectile типа)")]
        public GameObject ProjectilePrefab;

        [Header("Base Stats")]
        [Tooltip("Базовый урон")]
        public float BaseDamage = 10f;
        
        [Tooltip("Базовая стоимость маны")]
        public float BaseManaCost = 5f;
        
        [Tooltip("Базовый кулдаун (секунды)")]
        public float BaseCooldown = 0.5f;

        [Header("Projectile Stats")]
        [Tooltip("Скорость снаряда (для Projectile)")]
        public float ProjectileSpeed = 12f;
        
        [Tooltip("Дальность (время жизни снаряда)")]
        public float Range = 10f;
        
        [Tooltip("Пробитие (сколько врагов может пройти снаряд)")]
        public int Pierce = 0;

        [Header("Tags")]
        [Tooltip("Теги для фильтрации бафами")]
        public List<SpellTag> Tags = new List<SpellTag> { SpellTag.Arcane, SpellTag.Projectile };

        /// <summary>
        /// Проверка, имеет ли заклинание определенный тег
        /// </summary>
        public bool HasTag(SpellTag tag)
        {
            return Tags.Contains(tag);
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = name;
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = name;
            }

            BaseDamage = Mathf.Max(0f, BaseDamage);
            BaseManaCost = Mathf.Max(0f, BaseManaCost);
            BaseCooldown = Mathf.Max(0f, BaseCooldown);
            ProjectileSpeed = Mathf.Max(0.1f, ProjectileSpeed);
            Range = Mathf.Max(0.1f, Range);
            Pierce = Mathf.Max(0, Pierce);
        }
    }
}
