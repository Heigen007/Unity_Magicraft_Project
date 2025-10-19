using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.Combat
{
    /// <summary>
    /// Режим стакания бафов
    /// </summary>
    public enum StackingMode
    {
        Additive,        // Аддитивное сложение (1.2 + 1.3 = 2.5)
        Multiplicative,  // Мультипликативное умножение (1.2 * 1.3 = 1.56)
        Override         // Перезапись (берется последнее значение)
    }

    /// <summary>
    /// ScriptableObject для описания бафа/модификатора
    /// Баф влияет на все заклинания СЛЕВА от себя в посохе
    /// </summary>
    [CreateAssetMenu(fileName = "New Buff", menuName = "Magicraft/Buff", order = 2)]
    public class BuffSO : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Уникальный идентификатор бафа")]
        public string Id;
        
        [Tooltip("Отображаемое название")]
        public string DisplayName;
        
        [Tooltip("Иконка для UI")]
        public Sprite Icon;
        
        [TextArea(3, 5)]
        [Tooltip("Описание эффекта бафа")]
        public string Description;

        [Header("Modifiers - Multiplicative")]
        [Tooltip("Множитель урона (1.0 = без изменений, 1.2 = +20%)")]
        [Range(0.1f, 5f)]
        public float DamageMultiplier = 1.0f;
        
        [Tooltip("Множитель стоимости маны (0.8 = -20%)")]
        [Range(0f, 2f)]
        public float ManaCostMultiplier = 1.0f;
        
        [Tooltip("Множитель кулдауна (0.9 = -10%)")]
        [Range(0.1f, 2f)]
        public float CooldownMultiplier = 1.0f;
        
        [Tooltip("Множитель скорости снаряда")]
        [Range(0.5f, 3f)]
        public float ProjectileSpeedMultiplier = 1.0f;

        [Header("Modifiers - Additive")]
        [Tooltip("Добавочное пробитие")]
        public int AddPierce = 0;
        
        [Tooltip("Добавочный шанс крита (0.1 = +10%)")]
        [Range(0f, 1f)]
        public float AddCritChance = 0f;
        
        [Tooltip("Множитель крит урона (2.0 = x2 урона при крите)")]
        [Range(1f, 5f)]
        public float CritMultiplier = 2.0f;

        [Header("Filter")]
        [Tooltip("Фильтр по тегам (пустой список = влияет на все заклинания)")]
        public List<SpellTag> AffectedTags = new List<SpellTag>();

        [Header("Stacking")]
        [Tooltip("Режим стакания с другими бафами")]
        public StackingMode StackingMode = StackingMode.Multiplicative;

        /// <summary>
        /// Проверка, может ли баф влиять на заклинание с данными тегами
        /// </summary>
        public bool CanAffectSpell(SpellSO spell)
        {
            // Если список пуст - баф влияет на все заклинания
            if (AffectedTags == null || AffectedTags.Count == 0)
            {
                return true;
            }

            // Проверяем, есть ли хотя бы один общий тег
            foreach (SpellTag tag in AffectedTags)
            {
                if (spell.HasTag(tag))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Есть ли хотя бы один модификатор отличный от дефолтного
        /// </summary>
        public bool HasAnyModifier()
        {
            return !Mathf.Approximately(DamageMultiplier, 1f) ||
                   !Mathf.Approximately(ManaCostMultiplier, 1f) ||
                   !Mathf.Approximately(CooldownMultiplier, 1f) ||
                   !Mathf.Approximately(ProjectileSpeedMultiplier, 1f) ||
                   AddPierce != 0 ||
                   AddCritChance > 0f ||
                   !Mathf.Approximately(CritMultiplier, 2f);
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

            DamageMultiplier = Mathf.Max(0.1f, DamageMultiplier);
            ManaCostMultiplier = Mathf.Max(0f, ManaCostMultiplier);
            CooldownMultiplier = Mathf.Max(0.1f, CooldownMultiplier);
            ProjectileSpeedMultiplier = Mathf.Max(0.5f, ProjectileSpeedMultiplier);
            AddPierce = Mathf.Max(0, AddPierce);
            AddCritChance = Mathf.Clamp01(AddCritChance);
            CritMultiplier = Mathf.Max(1f, CritMultiplier);
        }
    }
}
