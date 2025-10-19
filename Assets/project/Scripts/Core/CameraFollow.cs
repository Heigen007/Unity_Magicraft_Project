using UnityEngine;

namespace Magicraft.Core
{
    /// <summary>
    /// Камера следует за игроком с плавным сглаживанием
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Цель для следования (обычно игрок)")]
        [SerializeField] private Transform target;

        [Header("Follow Settings")]
        [Tooltip("Скорость следования (чем меньше, тем плавнее)")]
        [SerializeField] private float smoothSpeed = 0.125f;

        [Tooltip("Смещение камеры относительно цели")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        [Header("Bounds (Optional)")]
        [Tooltip("Ограничить движение камеры в пределах границ")]
        [SerializeField] private bool useBounds = false;

        [Tooltip("Минимальная позиция камеры")]
        [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);

        [Tooltip("Максимальная позиция камеры")]
        [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

        private void LateUpdate()
        {
            if (target == null)
            {
                // Попытаться найти игрока по тегу
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
                return;
            }

            FollowTarget();
        }

        /// <summary>
        /// Следование за целью
        /// </summary>
        private void FollowTarget()
        {
            // Желаемая позиция
            Vector3 desiredPosition = target.position + offset;

            // Применить границы
            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            // Плавное перемещение
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        /// <summary>
        /// Установить цель для следования
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Мгновенно переместиться к цели (без сглаживания)
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = desiredPosition;
        }

        private void OnDrawGizmosSelected()
        {
            if (!useBounds) return;

            // Визуализация границ
            Gizmos.color = Color.yellow;
            Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, 0f);
            Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, 0f);
            Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, 0f);
            Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, 0f);

            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }
    }
}
