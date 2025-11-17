using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Magicraft.Rewards
{
    /// <summary>
    /// Компонент карточки награды
    /// </summary>
    public class RewardCard : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Изображение иконки")]
        [SerializeField] private Image iconImage;

        [Tooltip("Текст названия")]
        [SerializeField] private TextMeshProUGUI nameText;

        [Tooltip("Текст описания")]
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Tooltip("Текст типа (Spell/Buff)")]
        [SerializeField] private TextMeshProUGUI typeText;

        [Tooltip("Кнопка выбора")]
        [SerializeField] private Button selectButton;

        [Header("Colors")]
        [Tooltip("Цвет рамки для заклинаний")]
        [SerializeField] private Color spellColor = new Color(0.3f, 0.5f, 1f); // Синий

        [Tooltip("Цвет рамки для бафов")]
        [SerializeField] private Color buffColor = new Color(1f, 0.5f, 0.3f); // Оранжевый

        [Tooltip("Рамка карточки (для смены цвета)")]
        [SerializeField] private Image borderImage;

        // Текущая награда
        private RewardOption currentReward;

        // Событие выбора
        public System.Action<RewardOption> OnCardSelected;

        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnSelectClicked);
            }
        }

        /// <summary>
        /// Установить данные награды
        /// </summary>
        public void SetReward(RewardOption reward)
        {
            currentReward = reward;

            if (reward == null || !reward.IsValid())
            {
                Debug.LogWarning("[RewardCard] Invalid reward!");
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            // Обновить визуал
            UpdateVisuals();
        }

        /// <summary>
        /// Обновить визуальное представление
        /// </summary>
        private void UpdateVisuals()
        {
            // Название
            if (nameText != null)
            {
                nameText.text = currentReward.GetName();
            }

            // Описание
            if (descriptionText != null)
            {
                descriptionText.text = currentReward.GetDescription();
            }

            // Тип
            if (typeText != null)
            {
                typeText.text = currentReward.IsSpell ? "SPELL" : "BUFF";
            }

            // Цвет рамки
            if (borderImage != null)
            {
                borderImage.color = currentReward.IsSpell ? spellColor : buffColor;
            }

            // Иконка (если есть поле icon в SO)
            if (iconImage != null)
            {
                Sprite icon = GetIconFromReward();
                if (icon != null)
                {
                    iconImage.sprite = icon;
                    iconImage.enabled = true;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }
        }

        /// <summary>
        /// Получить иконку из ScriptableObject через рефлексию
        /// </summary>
        private Sprite GetIconFromReward()
        {
            if (currentReward == null || currentReward.Data == null) return null;

            var iconField = currentReward.Data.GetType().GetField("icon");
            if (iconField != null)
            {
                return iconField.GetValue(currentReward.Data) as Sprite;
            }

            return null;
        }

        /// <summary>
        /// Обработка клика по кнопке
        /// </summary>
        private void OnSelectClicked()
        {
            Debug.Log($"[RewardCard] Selected: {currentReward.GetName()}");
            OnCardSelected?.Invoke(currentReward);
        }

        /// <summary>
        /// Включить/выключить интерактивность
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            if (selectButton != null)
            {
                selectButton.interactable = interactable;
            }
        }

        private void OnDestroy()
        {
            if (selectButton != null)
            {
                selectButton.onClick.RemoveListener(OnSelectClicked);
            }
        }
    }
}
