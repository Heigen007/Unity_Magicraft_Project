using UnityEngine;

namespace Magicraft.Util
{
    /// <summary>
    /// Полезные extension-методы для Unity типов
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Получить 2D направление от одной точки к другой (нормализованное)
        /// </summary>
        public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }

        /// <summary>
        /// Получить 2D направление от Transform к другой точке
        /// </summary>
        public static Vector2 DirectionTo(this Transform from, Vector2 to)
        {
            return ((Vector2)from.position).DirectionTo(to);
        }

        /// <summary>
        /// Получить расстояние между двумя точками в 2D
        /// </summary>
        public static float DistanceTo(this Vector2 from, Vector2 to)
        {
            return Vector2.Distance(from, to);
        }

        /// <summary>
        /// Получить расстояние от Transform до точки в 2D
        /// </summary>
        public static float DistanceTo(this Transform from, Vector2 to)
        {
            return Vector2.Distance(from.position, to);
        }

        /// <summary>
        /// Установить X компоненту Vector2
        /// </summary>
        public static Vector2 WithX(this Vector2 v, float x)
        {
            return new Vector2(x, v.y);
        }

        /// <summary>
        /// Установить Y компоненту Vector2
        /// </summary>
        public static Vector2 WithY(this Vector2 v, float y)
        {
            return new Vector2(v.x, y);
        }

        /// <summary>
        /// Конвертировать угол в направление (2D)
        /// </summary>
        public static Vector2 AngleToDirection(float angleDegrees)
        {
            float angleRad = angleDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        /// <summary>
        /// Конвертировать направление в угол (2D)
        /// </summary>
        public static float DirectionToAngle(this Vector2 direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Clamp значение с минимумом 0
        /// </summary>
        public static float ClampMin(this float value, float min = 0f)
        {
            return Mathf.Max(value, min);
        }

        /// <summary>
        /// Проверка, находится ли точка в пределах экрана
        /// </summary>
        public static bool IsInScreenBounds(this Vector2 worldPosition, Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            
            Vector3 viewportPos = cam.WorldToViewportPoint(worldPosition);
            return viewportPos.x >= 0 && viewportPos.x <= 1 && 
                   viewportPos.y >= 0 && viewportPos.y <= 1;
        }
    }
}
