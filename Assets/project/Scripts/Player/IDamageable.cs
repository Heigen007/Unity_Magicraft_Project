using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Интерфейс для всех объектов, которые могут получать урон
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Применить урон к объекту
        /// </summary>
        /// <param name="amount">Количество урона</param>
        /// <param name="source">Источник урона (опционально)</param>
        void ApplyDamage(float amount, GameObject source = null);

        /// <summary>
        /// Проверка, жив ли объект
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Transform объекта (для позиции, эффектов и т.д.)
        /// </summary>
        Transform Transform { get; }
    }
}
