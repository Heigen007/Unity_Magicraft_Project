using UnityEngine;
using UnityEngine.InputSystem;
using Magicraft.Combat;
using Magicraft.Util;

namespace Magicraft.Player
{
    /// <summary>
    /// Контроллер игрока: движение, поворот к курсору мыши
    /// Реализует ICaster для интеграции с Wand
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, ICaster
    {
        [Header("Movement")]
        [Tooltip("Скорость движения игрока")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Tooltip("Сглаживание движения (0 = мгновенное, 1 = очень плавное)")]
        [SerializeField] private float movementSmoothing = 0.05f;

        [Header("Rotation")]
        [Tooltip("Поворачивать ли игрока к курсору мыши (ОТКЛЮЧЕНО - только посох!")]
        [SerializeField] private bool rotateTowardsMouse = false; // ВСЕГДА FALSE

        [Tooltip("Поворачивать ли посох к курсору мыши")]
        [SerializeField] private bool rotateWandTowardsMouse = true;

        [Tooltip("Скорость поворота (0 = мгновенный, выше = плавнее)")]
        [SerializeField] private float rotationSpeed = 0f;

        [Header("Wand Visual")]
        [Tooltip("Визуал посоха, который поворачивается к мыши")]
        [SerializeField] private Transform wandVisual;

        [Tooltip("Радиус вращения посоха вокруг игрока (расстояние от центра игрока до точки хвата посоха)")]
        [SerializeField] private float wandOrbitRadius = 0.5f;
        
        [Tooltip("Смещение посоха вперед от точки хвата (длина посоха впереди руки)")]
        [SerializeField] private float wandForwardOffset = 0.8f;

    // Компоненты
    private Rigidbody2D rb;
    private Camera mainCamera;
    private ManaComponent manaComponent;
    private Animator animator;

        // Input
        private Vector2 moveInput;
        private Vector2 currentVelocity;

        // Свойства
    public Vector2 MoveDirection => moveInput;
    public Vector2 AimDirection { get; private set; }
        
    // ICaster: Muzzle transform (can be assigned in Inspector)
    [Header("Caster")]
    [Tooltip("Точка выхода снаряда (пустой объект, дочерний к игроку)")]
    [SerializeField] private Transform muzzle;

    public Transform Muzzle => muzzle != null ? muzzle : transform;
        public bool IsMoving => moveInput.sqrMagnitude > 0.01f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;

            // Получить ManaComponent (если есть)
            manaComponent = GetComponent<ManaComponent>();

            // Получить Animator (если есть)
            animator = GetComponent<Animator>();

            // Настройка Rigidbody2D для top-down игры
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            // ВАЖНО: НЕ меняем bodyType - оставляем Dynamic для движения через velocity
            // Столкновения с врагами будут обрабатываться через коллайдер настройки
        }

        private void Update()
        {
            UpdateAimDirection();
            UpdateRotation();
            UpdateAnimation();
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

        #region ICaster Implementation
        public bool TrySpendMana(float amount)
        {
            if (manaComponent == null) return true; // если мана нет - пропустим траты
            return manaComponent.TrySpend(amount);
        }

        public void OnSpellCasted(CastContext context)
        {
            // Hook для визуальных/аудио эффектов. Пока пусто.
        }
        #endregion

        /// <summary>
        /// Поворот игрока и/или посоха в направлении курсора
        /// </summary>
        private void UpdateRotation()
        {
            if (AimDirection.sqrMagnitude < 0.01f)
            {
                return;
            }

            // Вычислить угол к курсору
            float targetAngle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;

            // Поворот игрока (обычно выключен)
            if (rotateTowardsMouse)
            {
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

            // Поворот посоха (обычно включен)
            if (rotateWandTowardsMouse && wandVisual != null)
            {
                // 1. Сначала поворачиваем посох в направлении мыши
                if (rotationSpeed > 0f)
                {
                    float currentWandAngle = wandVisual.eulerAngles.z;
                    float smoothWandAngle = Mathf.LerpAngle(currentWandAngle, targetAngle, Time.deltaTime * rotationSpeed);
                    wandVisual.rotation = Quaternion.Euler(0f, 0f, smoothWandAngle);
                }
                else
                {
                    wandVisual.rotation = Quaternion.Euler(0f, 0f, targetAngle);
                }
                
                // 2. Теперь позиционируем посох:
                // - Точка хвата находится на расстоянии wandOrbitRadius от игрока в направлении мыши
                // - Сам посох смещен назад на wandForwardOffset, чтобы точка хвата была правильной
                Vector3 gripPosition = transform.position + (AimDirection * wandOrbitRadius).ToVector3();
                Vector3 wandPosition = gripPosition - (AimDirection * wandForwardOffset).ToVector3();
                
                wandVisual.position = wandPosition;
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

        /// <summary>
        /// Обновление анимации
        /// </summary>
        private void UpdateAnimation()
        {
            if (animator == null)
            {
                return; // Animator не назначен, пропускаем
            }

            bool isMoving = IsMoving;
            animator.SetBool("isMoving", isMoving); // Маленькая буква - как в вашей анимации
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
