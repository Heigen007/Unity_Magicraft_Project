using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.Rewards
{
    /// <summary>
    /// UI выбора наград: показывает карточки и обрабатывает выбор
    /// </summary>
    public class RewardChoiceUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Панель с карточками")]
        [SerializeField] private GameObject panel;

        [Tooltip("Контейнер для карточек")]
        [SerializeField] private Transform cardsContainer;

        [Tooltip("Префаб карточки награды")]
        [SerializeField] private GameObject cardPrefab;

        [Header("Settings")]
        [Tooltip("Паузить игру при показе наград")]
        [SerializeField] private bool pauseGameOnShow = true;

        // Текущие карточки
        private List<RewardCard> activeCards = new List<RewardCard>();

        // Событие выбора награды
        public System.Action<RewardOption> OnRewardSelected;

        private void Awake()
        {
            // Скрыть панель по умолчанию
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Показать выбор наград
        /// </summary>
        public void ShowRewards(List<RewardOption> rewards)
        {
            if (rewards == null || rewards.Count == 0)
            {
                Debug.LogWarning("[RewardChoiceUI] No rewards to show!");
                return;
            }

            Debug.Log($"[RewardChoiceUI] Showing {rewards.Count} reward options");

            // Очистить старые карточки
            ClearCards();

            // Создать новые карточки
            foreach (var reward in rewards)
            {
                CreateCard(reward);
            }

            // Показать панель
            if (panel != null)
            {
                panel.SetActive(true);
            }

            // Пауза
            if (pauseGameOnShow)
            {
                Time.timeScale = 0f;
                Debug.Log("[RewardChoiceUI] Game paused");
            }
        }

        /// <summary>
        /// Создать карточку награды
        /// </summary>
        private void CreateCard(RewardOption reward)
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[RewardChoiceUI] Card prefab not assigned!");
                return;
            }

            if (cardsContainer == null)
            {
                Debug.LogError("[RewardChoiceUI] Cards container not assigned!");
                return;
            }

            // Создать объект
            GameObject cardObj = Instantiate(cardPrefab, cardsContainer);
            RewardCard card = cardObj.GetComponent<RewardCard>();

            if (card == null)
            {
                Debug.LogError("[RewardChoiceUI] Card prefab doesn't have RewardCard component!");
                Destroy(cardObj);
                return;
            }

            // Установить данные
            card.SetReward(reward);

            // Подписаться на выбор
            card.OnCardSelected += OnCardSelected;

            // Добавить в список
            activeCards.Add(card);
        }

        /// <summary>
        /// Обработка выбора карточки
        /// </summary>
        private void OnCardSelected(RewardOption reward)
        {
            Debug.Log($"[RewardChoiceUI] Reward selected: {reward.GetName()}");

            // Отключить все карточки
            foreach (var card in activeCards)
            {
                card.SetInteractable(false);
            }

            // Вызвать событие
            OnRewardSelected?.Invoke(reward);

            // Скрыть панель
            HideRewards();
        }

        /// <summary>
        /// Скрыть панель наград
        /// </summary>
        public void HideRewards()
        {
            // Скрыть панель
            if (panel != null)
            {
                panel.SetActive(false);
            }

            // Снять паузу
            if (pauseGameOnShow)
            {
                Time.timeScale = 1f;
                Debug.Log("[RewardChoiceUI] Game resumed");
            }

            // Очистить карточки
            ClearCards();
        }

        /// <summary>
        /// Очистить все карточки
        /// </summary>
        private void ClearCards()
        {
            foreach (var card in activeCards)
            {
                if (card != null)
                {
                    card.OnCardSelected -= OnCardSelected;
                    Destroy(card.gameObject);
                }
            }

            activeCards.Clear();
        }

        /// <summary>
        /// Проверка, показана ли панель
        /// </summary>
        public bool IsShowing()
        {
            return panel != null && panel.activeSelf;
        }

        private void OnDestroy()
        {
            // Снять паузу на всякий случай
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f;
            }

            ClearCards();
        }
    }
}
