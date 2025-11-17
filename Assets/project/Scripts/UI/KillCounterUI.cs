using UnityEngine;
using TMPro;

namespace Magicraft.UI
{
    /// <summary>
    /// UI счётчика убийств
    /// </summary>
    public class KillCounterUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Текст счётчика")]
        [SerializeField] private TextMeshProUGUI counterText;

        [Tooltip("Текст прогресса до награды")]
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("References")]
        [Tooltip("RewardSystem для получения данных")]
        [SerializeField] private Rewards.RewardSystem rewardSystem;

        [Header("Format")]
        [Tooltip("Формат текста счётчика (0 = количество убийств)")]
        [SerializeField] private string counterFormat = "Kills: {0}";

        [Tooltip("Формат текста прогресса (0 = убийств до награды)")]
        [SerializeField] private string progressFormat = "Next reward: {0} kills";

        private void Awake()
        {
            // Найти RewardSystem автоматически
            if (rewardSystem == null)
            {
                rewardSystem = FindFirstObjectByType<Rewards.RewardSystem>();
            }
        }

        private void Start()
        {
            if (rewardSystem != null)
            {
                // Подписка на события
                rewardSystem.OnKillCountChanged += UpdateDisplay;

                // Начальное обновление
                UpdateDisplay(0);
            }
            else
            {
                Debug.LogWarning("[KillCounterUI] RewardSystem not found!");
            }
        }

        /// <summary>
        /// Обновить отображение
        /// </summary>
        private void UpdateDisplay(int totalKills)
        {
            // Обновить счётчик
            if (counterText != null)
            {
                counterText.text = string.Format(counterFormat, totalKills);
            }

            // Обновить прогресс
            if (progressText != null && rewardSystem != null)
            {
                int killsUntilReward = rewardSystem.GetKillsUntilNextReward();
                progressText.text = string.Format(progressFormat, killsUntilReward);
            }
        }

        private void OnDestroy()
        {
            if (rewardSystem != null)
            {
                rewardSystem.OnKillCountChanged -= UpdateDisplay;
            }
        }
    }
}
