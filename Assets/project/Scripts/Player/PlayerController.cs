using UnityEngine;
using UnityEngine.InputSystem;

namespace Magicraft.Player
{
    /// <summary>
    /// Контроллер игрока: движение, поворот к курсору мыши
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Скорость движения игрока")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Tooltip("Сглаживание движения (0 = мгновенное, 1 = очень плавное)")]
        [SerializeField] private float movementSmoothing = 0.05f;

        [Header("Rotation")]
        [Tooltip("Поворачивать ли игрока к курсору мыши")]
        [SerializeField] private bool rotateTowardsMouse = true;

        [Tooltip("Скорость поворота (0 = мгновенный, выше = плавнее)")]
        [SerializeField] private float rotationSpeed = 0f;

        // Компоненты
        private Rigidbody2D rb;
        private Camera mainCamera;

        // Input
        private Vector2 moveInput;
        private Vector2 currentVelocity;

        // Свойства
        public Vector2 MoveDirection => moveInput;
        public Vector2 AimDirection { get; private set; }
        public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;

            // Настройка Rigidbody2D для top-down игры
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void Update()
        {
            UpdateAimDirection();
            UpdateRotation();
        }

        private void FixedUpdate()
        {
            Move();
        }

        /// <summary>
        /// Обработка движения (вызывается из Input System)
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Движение игрока
        /// </summary>
        private void Move()
        {
            if (!IsMoving)
            {
                // Плавная остановка
                rb.linearVelocity = Vector2.SmoothDamp(
                    rb.linearVelocity, 
                    Vector2.zero, 
                    ref currentVelocity, 
                    movementSmoothing
                );
                return;
            }

            // Целевая скорость
            Vector2 targetVelocity = moveInput.normalized * moveSpeed;

            // Плавное движение или мгновенное
            if (movementSmoothing > 0.001f)
            {
                rb.linearVelocity = Vector2.SmoothDamp(
                    rb.linearVelocity,
                    targetVelocity,
                    ref currentVelocity,
                    movementSmoothing
                );
            }
            else
            {
                rb.linearVelocity = targetVelocity;
            }
        }

        /// <summary>
        /// Обновление направления прицеливания к курсору мыши
        /// </summary>
        private void UpdateAimDirection()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null) return;
            }

            // Получить позицию мыши в мировых координатах
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue(); // НОВЫЙ Input System
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            // Вычислить направление от игрока к мыши
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            AimDirection = direction;
        }

        /// <summary>
        /// Поворот игрока в направлении курсора
        /// </summary>
        private void UpdateRotation()
        {
            if (!rotateTowardsMouse || AimDirection.sqrMagnitude < 0.01f)
            {
                return;
            }

            // Вычислить угол к курсору
            float targetAngle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;

            // Применить поворот (мгновенный или плавный)
            if (rotationSpeed > 0f)
            {
                float currentAngle = transform.eulerAngles.z;
                float smoothAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Euler(0f, 0f, smoothAngle);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            }
        }

        /// <summary>
        /// Установить скорость движения (для бафов/дебафов)
        /// </summary>
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = Mathf.Max(0f, speed);
        }

        /// <summary>
        /// Остановить движение игрока
        /// </summary>
        public void Stop()
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            currentVelocity = Vector2.zero;
        }

        private void OnDrawGizmosSelected()
        {
            // Визуализация направления прицеливания
            if (Application.isPlaying && AimDirection.sqrMagnitude > 0.01f)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, AimDirection * 2f);
            }
        }
    }
}
