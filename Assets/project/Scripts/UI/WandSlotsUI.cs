using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Magicraft.UI
{
    /// <summary>
    /// UI отображение слотов посоха
    /// Показывает текущие заклинания и бафы в посохе
    /// </summary>
    public class WandSlotsUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Посох для отслеживания")]
        [SerializeField] private Combat.Wand wand;

        [Header("UI Elements")]
        [Tooltip("Префаб UI слота")]
        [SerializeField] private GameObject slotPrefab;

        [Tooltip("Контейнер для слотов (Horizontal Layout Group)")]
        [SerializeField] private Transform slotsContainer;

        [Header("Styling")]
        [Tooltip("Цвет заклинания")]
        [SerializeField] private Color spellColor = new Color(0.3f, 0.6f, 1f); // Синий

        [Tooltip("Цвет бафа")]
        [SerializeField] private Color buffColor = new Color(1f, 0.8f, 0.2f); // Золотой

        [Tooltip("Цвет активного слота")]
        [SerializeField] private Color activeColor = Color.white;

        [Tooltip("Цвет неактивного слота")]
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f);

        // Слоты UI
        private List<SlotUI> slotUIElements = new List<SlotUI>();

        private void Awake()
        {
            if (wand == null)
            {
                wand = FindFirstObjectByType<Combat.Wand>();
            }
        }

        private void OnEnable()
        {
            if (wand != null)
            {
                wand.OnSlotAdded += OnSlotAdded;
                wand.OnSlotRemoved += OnSlotRemoved;
                wand.OnSlotChanged += OnSlotChanged;

                // Инициализация
                RefreshAllSlots();
            }
        }

        private void OnDisable()
        {
            if (wand != null)
            {
                wand.OnSlotAdded -= OnSlotAdded;
                wand.OnSlotRemoved -= OnSlotRemoved;
                wand.OnSlotChanged -= OnSlotChanged;
            }
        }

        /// <summary>
        /// Обновить все слоты
        /// </summary>
        private void RefreshAllSlots()
        {
            // Очистить существующие
            ClearSlots();

            if (wand == null) return;

            // Создать UI для каждого слота
            var slots = wand.GetAllSlots();
            for (int i = 0; i < slots.Count; i++)
            {
                CreateSlotUI(i, slots[i]);
            }

            UpdateActiveSlot();
        }

        /// <summary>
        /// Создать UI элемент слота
        /// </summary>
        private void CreateSlotUI(int index, Combat.WandSlot slot)
        {
            if (slotPrefab == null || slotsContainer == null)
            {
                Debug.LogError("[WandSlotsUI] SlotPrefab or SlotsContainer is null!");
                return;
            }

            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            SlotUI slotUI = new SlotUI
            {
                GameObject = slotObj,
                Icon = slotObj.transform.Find("Icon")?.GetComponent<Image>(),
                Text = slotObj.transform.Find("Text")?.GetComponent<TextMeshProUGUI>(),
                Background = slotObj.GetComponent<Image>(),
                Index = index
            };

            // Установить данные
            UpdateSlotUI(slotUI, slot, false);

            slotUIElements.Add(slotUI);
        }

        /// <summary>
        /// Обновить UI слота
        /// </summary>
        private void UpdateSlotUI(SlotUI slotUI, Combat.WandSlot slot, bool isActive)
        {
            if (slotUI.Icon != null)
            {
                slotUI.Icon.sprite = slot.GetIcon();
                slotUI.Icon.enabled = slot.GetIcon() != null;
            }

            if (slotUI.Text != null)
            {
                slotUI.Text.text = slot.GetDisplayName();
            }

            if (slotUI.Background != null)
            {
                // Цвет фона в зависимости от типа
                Color bgColor = slot.IsSpell ? spellColor : buffColor;
                slotUI.Background.color = isActive ? activeColor : bgColor;
            }
        }

        /// <summary>
        /// Обновить активный слот
        /// </summary>
        private void UpdateActiveSlot()
        {
            if (wand == null) return;

            for (int i = 0; i < slotUIElements.Count; i++)
            {
                bool isActive = i == wand.CurrentSlotIndex;
                var slot = wand.GetSlot(i);
                UpdateSlotUI(slotUIElements[i], slot, isActive);
            }
        }

        /// <summary>
        /// Очистить все слоты
        /// </summary>
        private void ClearSlots()
        {
            foreach (var slotUI in slotUIElements)
            {
                if (slotUI.GameObject != null)
                {
                    Destroy(slotUI.GameObject);
                }
            }
            slotUIElements.Clear();
        }

        // Обработчики событий
        private void OnSlotAdded(Combat.WandSlot slot)
        {
            RefreshAllSlots();
        }

        private void OnSlotRemoved(int index)
        {
            RefreshAllSlots();
        }

        private void OnSlotChanged(int newIndex)
        {
            UpdateActiveSlot();
        }

        /// <summary>
        /// Вспомогательная структура для хранения UI элементов слота
        /// </summary>
        private class SlotUI
        {
            public GameObject GameObject;
            public Image Icon;
            public TextMeshProUGUI Text;
            public Image Background;
            public int Index;
        }
    }
}
