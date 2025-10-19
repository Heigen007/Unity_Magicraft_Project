using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Таймер для отслеживания кулдауна
    /// </summary>
    public class CooldownTimer
    {
        private float cooldownDuration;
        private float remainingTime;

        /// <summary>
        /// Готов ли таймер (кулдаун закончился)
        /// </summary>
        public bool IsReady => remainingTime <= 0f;

        /// <summary>
        /// Оставшееся время кулдауна
        /// </summary>
        public float Remaining => Mathf.Max(0f, remainingTime);

        /// <summary>
        /// Прогресс кулдауна (0 = только начат, 1 = закончен)
        /// </summary>
        public float Progress => cooldownDuration > 0f ? Mathf.Clamp01(1f - remainingTime / cooldownDuration) : 1f;

        /// <summary>
        /// Нормализованное оставшееся время (0 = закончен, 1 = только начат)
        /// </summary>
        public float Normalized => cooldownDuration > 0f ? Mathf.Clamp01(remainingTime / cooldownDuration) : 0f;

        /// <summary>
        /// Текущая длительность кулдауна
        /// </summary>
        public float Duration => cooldownDuration;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CooldownTimer()
        {
            remainingTime = 0f;
            cooldownDuration = 0f;
        }

        /// <summary>
        /// Запустить кулдаун
        /// </summary>
        /// <param name="duration">Длительность кулдауна в секундах</param>
        public void Start(float duration)
        {
            cooldownDuration = Mathf.Max(0f, duration);
            remainingTime = cooldownDuration;
        }

        /// <summary>
        /// Обновить таймер (вызывать каждый кадр)
        /// </summary>
        /// <param name="deltaTime">Прошедшее время</param>
        public void Update(float deltaTime)
        {
            if (remainingTime > 0f)
            {
                remainingTime -= deltaTime;
            }
        }

        /// <summary>
        /// Сбросить кулдаун (немедленно завершить)
        /// </summary>
        public void Reset()
        {
            remainingTime = 0f;
        }

        /// <summary>
        /// Установить кулдаун на готовность
        /// </summary>
        public void SetReady()
        {
            remainingTime = 0f;
        }
    }
}
