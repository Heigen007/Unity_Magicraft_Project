using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Magicraft.UI
{
    /// <summary>
    /// UI полоска маны
    /// Отображает текущую и максимальную ману
    /// </summary>
    public class ManaBar : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Компонент маны игрока")]
        [SerializeField] private Player.ManaComponent manaComponent;

        [Tooltip("Image для заполнения полоски (Fill Image)")]
        [SerializeField] private Image fillImage;

        [Tooltip("Текст с числовым значением (опционально)")]
        [SerializeField] private TextMeshProUGUI manaText;

        [Header("Visual Settings")]
        [Tooltip("Цвет полной маны")]
        [SerializeField] private Color fullColor = new Color(0.2f, 0.5f, 1f); // Синий

        [Tooltip("Цвет низкой маны")]
        [SerializeField] private Color lowColor = new Color(1f, 0.3f, 0.3f); // Красный

        [Tooltip("Порог низкой маны (процент)")]
        [SerializeField] private float lowManaThreshold = 0.3f;

        [Tooltip("Плавность анимации изменения")]
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Text Format")]
        [Tooltip("Показывать ли текст")]
        [SerializeField] private bool showText = true;

        [Tooltip("Формат текста: {0} = текущая, {1} = максимум")]
        [SerializeField] private string textFormat = "{0:0}/{1:0}";

        // Для плавной анимации
        private float targetFillAmount;
        private float currentFillAmount;

        private void Awake()
        {
            // Попытаться найти ManaComponent автоматически
            if (manaComponent == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    manaComponent = player.GetComponent<Player.ManaComponent>();
                }
            }

            // Валидация
            if (fillImage == null)
            {
                Debug.LogError("[ManaBar] Fill Image не назначен!", this);
            }
        }

        private void OnEnable()
        {
            if (manaComponent != null)
            {
                manaComponent.OnManaChanged += UpdateManaDisplay;
                
                // Инициализация
                UpdateManaDisplay(manaComponent.CurrentMana, manaComponent.MaxMana);
            }
        }

        private void OnDisable()
        {
            if (manaComponent != null)
            {
                manaComponent.OnManaChanged -= UpdateManaDisplay;
            }
        }

        private void Update()
        {
            // Плавная анимация заполнения
            if (fillImage != null)
            {
                currentFillAmount = Mathf.Lerp(
                    currentFillAmount,
                    targetFillAmount,
                    Time.deltaTime * smoothSpeed
                );

                fillImage.fillAmount = currentFillAmount;
            }
        }

        /// <summary>
        /// Обновить отображение маны
        /// </summary>
        private void UpdateManaDisplay(float current, float max)
        {
            if (max <= 0f) return;

            // Вычислить процент
            float percent = current / max;
            targetFillAmount = percent;

            // Обновить цвет (от красного к синему в зависимости от количества)
            if (fillImage != null)
            {
                fillImage.color = percent <= lowManaThreshold 
                    ? Color.Lerp(lowColor, fullColor, percent / lowManaThreshold)
                    : fullColor;
            }

            // Обновить текст
            if (showText && manaText != null)
            {
                manaText.text = string.Format(textFormat, current, max);
            }
        }

        /// <summary>
        /// Установить компонент маны вручную
        /// </summary>
        public void SetManaComponent(Player.ManaComponent mana)
        {
            // Отписаться от старого
            if (manaComponent != null)
            {
                manaComponent.OnManaChanged -= UpdateManaDisplay;
            }

            // Подписаться на новый
            manaComponent = mana;
            if (manaComponent != null)
            {
                manaComponent.OnManaChanged += UpdateManaDisplay;
                UpdateManaDisplay(manaComponent.CurrentMana, manaComponent.MaxMana);
            }
        }
    }
}
