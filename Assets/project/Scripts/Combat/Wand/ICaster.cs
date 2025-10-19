using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Интерфейс для объектов, которые могут кастовать заклинания
    /// </summary>
    public interface ICaster
    {
        /// <summary>
        /// Точка выхода снаряда (дуло)
        /// </summary>
        Transform Muzzle { get; }

        /// <summary>
        /// Направление прицеливания (нормализованный вектор)
        /// </summary>
        Vector2 AimDirection { get; }

        /// <summary>
        /// Попытка потратить ману
        /// </summary>
        /// <param name="amount">Количество маны для траты</param>
        /// <returns>true если мана была успешно потрачена</returns>
        bool TrySpendMana(float amount);

        /// <summary>
        /// Событие, вызываемое после успешного каста заклинания
        /// </summary>
        /// <param name="context">Контекст каста</param>
        void OnSpellCasted(CastContext context);
    }
}
