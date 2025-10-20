using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Magicraft.UI
{
    /// <summary>
    /// UI элемент одного слота посоха с поддержкой Drag & Drop
    /// </summary>
    public class WandSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Header("UI References")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image highlight;

        [Header("Colors")]
        [SerializeField] private Color spellColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color buffColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 0f, 0.3f);

        // Данные
        private Combat.WandSlot slotData;
        private int slotIndex;
        private WandEditorUI editor;

        // Drag состояние
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector2 originalPosition;
        private Transform originalParent;

        public int SlotIndex => slotIndex;
        public Combat.WandSlot SlotData => slotData;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvas = GetComponentInParent<Canvas>();

            if (highlight != null)
                highlight.enabled = false;
        }

        /// <summary>
        /// Инициализировать слот
        /// </summary>
        public void Initialize(Combat.WandSlot slot, int index, WandEditorUI editorUI)
        {
            slotData = slot;
            slotIndex = index;
            editor = editorUI;

            UpdateVisuals();
        }

        /// <summary>
        /// Обновить визуал
        /// </summary>
        public void UpdateVisuals(bool isActive = false)
        {
            // Иконка
            if (icon != null)
            {
                icon.sprite = slotData.GetIcon();
                icon.enabled = slotData.GetIcon() != null;
            }

            // Название
            if (nameText != null)
            {
                nameText.text = slotData.GetDisplayName();
            }

            // Цвет фона
            if (background != null)
            {
                Color bgColor = slotData.IsSpell ? spellColor : buffColor;
                background.color = isActive ? activeColor : bgColor;
            }
        }

        /// <summary>
        /// Показать подсветку
        /// </summary>
        public void ShowHighlight(bool show)
        {
            if (highlight != null)
            {
                highlight.enabled = show;
                highlight.color = highlightColor;
            }
        }

        // ===== DRAG & DROP HANDLERS =====

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (editor == null) return;

            originalPosition = rectTransform.anchoredPosition;
            originalParent = transform.parent;

            // Сделать полупрозрачным
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            // ВАЖНО: Отключить Layout Element чтобы не мешал перетаскиванию
            var layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = true;
            }

            editor.OnSlotBeginDrag(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canvas == null) return;

            // Следовать за курсором
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // ВАЖНО: Включить обратно Layout Element
            var layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = false;
            }

            if (editor != null)
            {
                editor.OnSlotEndDrag(this);
            }

            // НЕ нужно возвращать позицию - Layout Group сам всё расставит
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Проверить, что дропнули другой слот
            WandSlotUI draggedSlot = eventData.pointerDrag?.GetComponent<WandSlotUI>();
            if (draggedSlot != null && draggedSlot != this && editor != null)
            {
                editor.OnSlotDropped(draggedSlot, this);
            }
        }
    }
}
