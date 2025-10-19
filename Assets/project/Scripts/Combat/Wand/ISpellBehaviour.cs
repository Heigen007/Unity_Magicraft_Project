namespace Magicraft.Combat
{
    /// <summary>
    /// Интерфейс для поведения заклинания
    /// Определяет, как заклинание исполняется (снаряд, луч, АОЕ, и т.д.)
    /// </summary>
    public interface ISpellBehaviour
    {
        /// <summary>
        /// Исполнить заклинание с заданным контекстом
        /// </summary>
        /// <param name="context">Контекст каста с финальными параметрами</param>
        void Execute(CastContext context);
    }
}
