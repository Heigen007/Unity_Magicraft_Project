using UnityEngine;

namespace Magicraft.Core
{
    /// <summary>
    /// Базовый bootstrap класс для инициализации игровых систем
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Целевой FPS для игры")]
        [SerializeField] private int targetFrameRate = 60;
        
        [Tooltip("VSync (0 = выкл, 1 = вкл)")]
        [SerializeField] private int vSyncCount = 0;

        private void Awake()
        {
            InitializeGameSettings();
        }

        /// <summary>
        /// Инициализация базовых настроек игры
        /// </summary>
        private void InitializeGameSettings()
        {
            // Настройка FPS
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncCount;

            // Курсор видимый (для UI и прицеливания)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("[GameBootstrap] Game initialized successfully");
        }
    }
}
