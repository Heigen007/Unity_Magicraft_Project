using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Тип слота в посохе
    /// </summary>
    public enum SlotType
    {
        Spell,  // Заклинание
        Buff    // Модификатор
    }

    /// <summary>
    /// Структура слота в посохе
    /// Может содержать либо заклинание, либо баф
    /// </summary>
    [System.Serializable]
    public struct WandSlot
    {
        public SlotType Type;
        public SpellSO Spell;
        public BuffSO Buff;

        /// <summary>
        /// Пустой слот
        /// </summary>
        public static WandSlot Empty => new WandSlot
        {
            Type = SlotType.Spell,
            Spell = null,
            Buff = null
        };

        /// <summary>
        /// Создать слот с заклинанием
        /// </summary>
        public static WandSlot FromSpell(SpellSO spell)
        {
            return new WandSlot
            {
                Type = SlotType.Spell,
                Spell = spell,
                Buff = null
            };
        }

        /// <summary>
        /// Создать слот с бафом
        /// </summary>
        public static WandSlot FromBuff(BuffSO buff)
        {
            return new WandSlot
            {
                Type = SlotType.Buff,
                Spell = null,
                Buff = buff
            };
        }

        /// <summary>
        /// Проверка, пуст ли слот
        /// </summary>
        public bool IsEmpty => Spell == null && Buff == null;

        /// <summary>
        /// Проверка, является ли слот заклинанием
        /// </summary>
        public bool IsSpell => Type == SlotType.Spell && Spell != null;

        /// <summary>
        /// Проверка, является ли слот бафом
        /// </summary>
        public bool IsBuff => Type == SlotType.Buff && Buff != null;

        /// <summary>
        /// Получить отображаемое имя
        /// </summary>
        public string GetDisplayName()
        {
            if (IsSpell) return Spell.DisplayName;
            if (IsBuff) return Buff.DisplayName;
            return "Empty";
        }

        /// <summary>
        /// Получить иконку
        /// </summary>
        public Sprite GetIcon()
        {
            if (IsSpell) return Spell.Icon;
            if (IsBuff) return Buff.Icon;
            return null;
        }

        /// <summary>
        /// Очистить слот
        /// </summary>
        public void Clear()
        {
            Spell = null;
            Buff = null;
        }
    }
}
