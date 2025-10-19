using UnityEngine;

namespace Magicraft.UI
{
    /// <summary>
    /// Главный HUD канвас
    /// Управляет всеми элементами UI игрока
    /// </summary>
    public class HUDCanvas : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Полоска маны")]
        [SerializeField] private ManaBar manaBar;

        [Header("Auto-Setup")]
        [Tooltip("Автоматически искать игрока при старте")]
        [SerializeField] private bool autoFindPlayer = true;

        private void Start()
        {
            if (autoFindPlayer)
            {
                SetupHUD();
            }
        }

        /// <summary>
        /// Настроить HUD (связать с компонентами игрока)
        /// </summary>
        public void SetupHUD()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("[HUDCanvas] Игрок не найден! Убедитесь, что у объекта Player есть тег 'Player'");
                return;
            }

            // Настроить ManaBar
            if (manaBar != null)
            {
                var manaComponent = player.GetComponent<Player.ManaComponent>();
                if (manaComponent != null)
                {
                    manaBar.SetManaComponent(manaComponent);
                }
                else
                {
                    Debug.LogWarning("[HUDCanvas] У игрока нет ManaComponent!");
                }
            }

            Debug.Log("[HUDCanvas] HUD настроен успешно");
        }

        /// <summary>
        /// Показать HUD
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Скрыть HUD
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
