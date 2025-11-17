using UnityEngine;

namespace Magicraft.Rewards
{
    /// <summary>
    /// Вариант награды: может быть заклинанием или бафом
    /// </summary>
    [System.Serializable]
    public class RewardOption
    {
        public enum RewardType
        {
            Spell,
            Buff
        }

        [Tooltip("Тип награды")]
        public RewardType type;

        [Tooltip("ScriptableObject заклинания (если type = Spell)")]
        public ScriptableObject spellData;

        [Tooltip("ScriptableObject бафа (если type = Buff)")]
        public ScriptableObject buffData;

        // Удобные свойства
        public bool IsSpell => type == RewardType.Spell;
        public bool IsBuff => type == RewardType.Buff;
        public ScriptableObject Data => IsSpell ? spellData : buffData;

        /// <summary>
        /// Конструктор для заклинания
        /// </summary>
        public static RewardOption CreateSpell(ScriptableObject spell)
        {
            return new RewardOption
            {
                type = RewardType.Spell,
                spellData = spell,
                buffData = null
            };
        }

        /// <summary>
        /// Конструктор для бафа
        /// </summary>
        public static RewardOption CreateBuff(ScriptableObject buff)
        {
            return new RewardOption
            {
                type = RewardType.Buff,
                spellData = null,
                buffData = buff
            };
        }

        /// <summary>
        /// Получить название награды
        /// </summary>
        public string GetName()
        {
            if (Data == null) return "Unknown";
            return Data.name;
        }

        /// <summary>
        /// Получить описание награды
        /// </summary>
        public string GetDescription()
        {
            if (Data == null) return "No description";

            // Попытка получить описание через рефлексию
            var descField = Data.GetType().GetField("description");
            if (descField != null)
            {
                return descField.GetValue(Data) as string ?? "No description";
            }

            return "No description";
        }

        /// <summary>
        /// Проверка валидности
        /// </summary>
        public bool IsValid()
        {
            return Data != null;
        }
    }
}
