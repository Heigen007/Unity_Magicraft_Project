using UnityEngine;
using UnityEngine.InputSystem;

namespace Magicraft.Player
{
    /// <summary>
    /// Обработчик ввода для игрока
    /// Использует Input System напрямую через код (без .inputactions файла)
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInputHandler : MonoBehaviour
    {
        // Input Actions (создаются в коде)
        private InputAction moveAction;
        private InputAction fireAction;

        // Компоненты
        private PlayerController playerController;

        // События для других систем
        public System.Action OnFirePressed;
        public System.Action OnFireReleased;
        public bool IsFireHeld { get; private set; }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();

            // Создание Input Actions через код
            CreateInputActions();
        }

        private void OnEnable()
        {
            // Активировать actions
            moveAction?.Enable();
            fireAction?.Enable();
        }

        private void OnDisable()
        {
            // Деактивировать actions
            moveAction?.Disable();
            fireAction?.Disable();
        }

        private void OnDestroy()
        {
            // Очистка
            moveAction?.Dispose();
            fireAction?.Dispose();
        }

        /// <summary>
        /// Создать Input Actions через код
        /// </summary>
        private void CreateInputActions()
        {
            // Move Action: WASD + стрелки
            moveAction = new InputAction(
                name: "Move",
                type: InputActionType.Value,
                binding: "<Keyboard>/w",
                interactions: null,
                processors: null
            );

            // Добавление композитного биндинга для WASD
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            // Добавление биндинга для стрелок
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            // Подписка на события
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            // Fire Action: ЛКМ
            fireAction = new InputAction(
                name: "Fire",
                type: InputActionType.Button,
                binding: "<Mouse>/leftButton",
                interactions: null,
                processors: null
            );

            // Подписка на события
            fireAction.started += OnFireStarted;
            fireAction.canceled += OnFireCanceled;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnMove(context);
            }
        }

        private void OnFireStarted(InputAction.CallbackContext context)
        {
            IsFireHeld = true;
            OnFirePressed?.Invoke();
        }

        private void OnFireCanceled(InputAction.CallbackContext context)
        {
            IsFireHeld = false;
            OnFireReleased?.Invoke();
        }
    }
}
