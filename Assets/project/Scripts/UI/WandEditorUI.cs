using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Magicraft.UI
{
    /// <summary>
    /// Редактор посоха с Drag & Drop функционалом
    /// Позволяет перетаскивать слоты для изменения порядка
    /// </summary>
    public class WandEditorUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Посох для редактирования")]
        [SerializeField] private Combat.Wand wand;

        [Header("UI Elements")]
        [Tooltip("Префаб UI слота")]
        [SerializeField] private GameObject slotPrefab;

        [Tooltip("Контейнер для слотов")]
        [SerializeField] private Transform slotsContainer;

        [Header("Settings")]
        [Tooltip("Автоматически обновлять при изменениях")]
        [SerializeField] private bool autoRefresh = true;

        // UI слоты
        private List<WandSlotUI> slotUIList = new List<WandSlotUI>();
        private WandSlotUI currentDraggedSlot;

        private void Awake()
        {
            if (wand == null)
            {
                wand = FindFirstObjectByType<Combat.Wand>();
            }
        }

        private void OnEnable()
        {
            if (wand != null && autoRefresh)
            {
                wand.OnSlotAdded += OnWandSlotChanged;
                wand.OnSlotRemoved += OnWandSlotRemoved;

                RefreshUI();
            }
        }

        private void OnDisable()
        {
            if (wand != null)
            {
                wand.OnSlotAdded -= OnWandSlotChanged;
                wand.OnSlotRemoved -= OnWandSlotRemoved;
            }
        }

        /// <summary>
        /// Полное обновление UI
        /// </summary>
        public void RefreshUI()
        {
            ClearSlots();

            if (wand == null || slotsContainer == null)
                return;

            var slots = wand.GetAllSlots();
            for (int i = 0; i < slots.Count; i++)
            {
                CreateSlotUI(i, slots[i]);
            }
        }

        /// <summary>
        /// Создать UI элемент слота
        /// </summary>
        private void CreateSlotUI(int index, Combat.WandSlot slot)
        {
            if (slotPrefab == null || slotsContainer == null)
            {
                Debug.LogError("[WandEditorUI] SlotPrefab or SlotsContainer is null!");
                return;
            }

            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            WandSlotUI slotUI = slotObj.GetComponent<WandSlotUI>();

            if (slotUI == null)
            {
                slotUI = slotObj.AddComponent<WandSlotUI>();
            }

            slotUI.Initialize(slot, index, this);
            slotUIList.Add(slotUI);
        }

        /// <summary>
        /// Очистить все слоты
        /// </summary>
        private void ClearSlots()
        {
            foreach (var slotUI in slotUIList)
            {
                if (slotUI != null)
                    Destroy(slotUI.gameObject);
            }
            slotUIList.Clear();
        }

        /// <summary>
        /// Обновить индексы слотов
        /// </summary>
        private void UpdateSlotIndices()
        {
            var slots = wand.GetAllSlots();
            for (int i = 0; i < slotUIList.Count && i < slots.Count; i++)
            {
                slotUIList[i].Initialize(slots[i], i, this);
            }
        }

        // ===== DRAG & DROP CALLBACKS =====

        /// <summary>
        /// Вызывается когда начинается перетаскивание слота
        /// </summary>
        public void OnSlotBeginDrag(WandSlotUI slot)
        {
            currentDraggedSlot = slot;

            // Подсветить все слоты кроме текущего
            foreach (var slotUI in slotUIList)
            {
                if (slotUI != slot)
                    slotUI.ShowHighlight(true);
            }
        }

        /// <summary>
        /// Вызывается когда заканчивается перетаскивание
        /// </summary>
        public void OnSlotEndDrag(WandSlotUI slot)
        {
            currentDraggedSlot = null;

            // Убрать подсветку
            foreach (var slotUI in slotUIList)
            {
                slotUI.ShowHighlight(false);
            }
        }

        /// <summary>
        /// Вызывается когда слот дропнули на другой слот
        /// </summary>
        public void OnSlotDropped(WandSlotUI draggedSlot, WandSlotUI targetSlot)
        {
            if (wand == null || draggedSlot == null || targetSlot == null)
                return;

            int fromIndex = draggedSlot.SlotIndex;
            int toIndex = targetSlot.SlotIndex;

            if (fromIndex == toIndex)
                return;

            Debug.Log($"[WandEditorUI] Swapping slots: {fromIndex} ↔ {toIndex}");

            // Поменять местами слоты в Wand
            wand.SwapSlots(fromIndex, toIndex);

            // Поменять местами в иерархии UI (для Layout Group)
            int siblingIndex = targetSlot.transform.GetSiblingIndex();
            draggedSlot.transform.SetSiblingIndex(siblingIndex);

            // Обновить UI
            RefreshUI();

            Debug.Log("[WandEditorUI] Swap complete! Modifiers recalculated.");
        }

        // ===== EVENT HANDLERS =====

        private void OnWandSlotChanged(Combat.WandSlot slot)
        {
            if (autoRefresh)
                RefreshUI();
        }

        private void OnWandSlotRemoved(int index)
        {
            if (autoRefresh)
                RefreshUI();
        }

        // ===== PUBLIC METHODS =====

        /// <summary>
        /// Установить посох для редактирования
        /// </summary>
        public void SetWand(Combat.Wand newWand)
        {
            if (wand != null && autoRefresh)
            {
                wand.OnSlotAdded -= OnWandSlotChanged;
                wand.OnSlotRemoved -= OnWandSlotRemoved;
            }

            wand = newWand;

            if (wand != null && autoRefresh)
            {
                wand.OnSlotAdded += OnWandSlotChanged;
                wand.OnSlotRemoved += OnWandSlotRemoved;
            }

            RefreshUI();
        }
    }
}
