using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Magicraft.Combat
{
    /// <summary>
    /// Класс посоха
    /// Управляет слотами заклинаний/бафов, каст с кулдауном
    /// НА ЭТАПЕ 7: работает БЕЗ применения модификаторов (просто кастуем базовые заклинания)
    /// </summary>
    public class Wand : MonoBehaviour
    {
        [Header("Wand Configuration")]
        [Tooltip("ScriptableObject конфигурация посоха")]
        [SerializeField] private WandSO wandData;

        [Tooltip("Максимальное количество слотов")]
        [SerializeField] private int maxSlots = 6;

        [Header("References")]
        [Tooltip("ICaster (обычно Player)")]
        [SerializeField] private MonoBehaviour casterComponent;

        // Слоты
        private List<WandSlot> slots = new List<WandSlot>();

        // Системы
        private SpellExecutor spellExecutor;
        private CooldownTimer globalCooldown;
        private ICaster caster;
        private WandCasterWrapper wandCaster; // Временный wrapper для Этапа 7

        // Состояние
        private int currentSlotIndex = 0;
        private bool isReady = true;

        // События
        public System.Action<int> OnSlotChanged; // (newIndex)
        public System.Action<CastContext> OnSpellCasted; // (context)
        public System.Action<WandSlot> OnSlotAdded; // (slot)
        public System.Action<int> OnSlotRemoved; // (index)

        // Свойства
        public int SlotCount => slots.Count;
        public int MaxSlots => maxSlots;
        public int CurrentSlotIndex => currentSlotIndex;
        public WandSlot CurrentSlot => currentSlotIndex >= 0 && currentSlotIndex < slots.Count ? slots[currentSlotIndex] : WandSlot.Empty;
    public bool IsReady => isReady && (globalCooldown != null ? globalCooldown.IsReady : true);
    public float CooldownProgress => globalCooldown != null ? globalCooldown.Progress : 1f;

        private void Awake()
        {
            spellExecutor = new SpellExecutor();
            globalCooldown = new CooldownTimer();

            // Попытка определить ICaster:
            // 1) Если в инспекторе указан компонент и он реализует ICaster - используем его
            if (casterComponent != null && casterComponent is ICaster)
            {
                caster = casterComponent as ICaster;
                Debug.Log("[Wand] Caster found (inspector): " + caster);
            }
            else
            {
                // 2) Попытка найти ICaster на том же GameObject (например PlayerController)
                var localCaster = GetComponent<ICaster>();
                if (localCaster != null)
                {
                    caster = localCaster;
                    Debug.Log("[Wand] Caster found (GetComponent): " + caster);
                }
                else
                {
                    // 3) В крайнем случае - создаем wrapper для работы без ICaster (временное поведение)
                    wandCaster = new WandCasterWrapper(transform);
                    caster = wandCaster;
                    Debug.LogWarning("[Wand] No ICaster assigned. Using wrapper mode (no mana cost).");
                }
            }

            // Загрузить слоты из WandSO (если указан)
            if (wandData != null)
            {
                LoadFromWandData();
            }
        }

        private void Update()
        {
            // Defensive: ensure timer exists (in case Awake wasn't executed or initialization failed)
            if (globalCooldown == null)
            {
                globalCooldown = new CooldownTimer();
            }

            globalCooldown.Update(Time.deltaTime);
        }

        /// <summary>
        /// Загрузить слоты из WandSO
        /// </summary>
        private void LoadFromWandData()
        {
            slots.Clear();

            // Добавить базовые заклинания
            foreach (var spell in wandData.baseSpells)
            {
                if (spell != null)
                {
                    AddSlot(WandSlot.FromSpell(spell));
                }
            }

            maxSlots = wandData.maxSpellSlots;
        }

        /// <summary>
        /// Попытка каста текущего слота
        /// НА ЭТАПЕ 7: без применения модификаторов
        /// </summary>
        public bool TryCast()
        {
            if (!IsReady)
            {
                Debug.Log("[Wand] Not ready to cast (cooldown)");
                return false;
            }

            // Найти следующее заклинание для каста
            WandSlot spellSlot = FindNextSpellSlot();

            if (spellSlot.IsEmpty || !spellSlot.IsSpell)
            {
                Debug.Log("[Wand] No spell to cast!");
                return false;
            }

            // Построить CastContext БЕЗ модификаторов (Этап 7)
            if (spellSlot.Spell == null)
            {
                Debug.LogError("[Wand] SpellSO is null on the selected slot");
                return false;
            }

            CastContext context = BuildBasicCastContext(spellSlot.Spell);

            // Проверить ману (если есть caster)
            if (caster != null)
            {
                if (!caster.TrySpendMana(context.ManaCost))
                {
                    Debug.Log("[Wand] Not enough mana!");
                    return false;
                }
            }

            // Исполнить заклинание
            Debug.Log($"[Wand] Casting: {spellSlot.Spell.DisplayName}");
            if (spellExecutor == null)
            {
                spellExecutor = new SpellExecutor();
            }

            spellExecutor.Execute(context);

            // Запустить кулдаун
            globalCooldown.Start(context.Cooldown);

            // События
            OnSpellCasted?.Invoke(context);
            if (caster != null)
            {
                caster.OnSpellCasted(context);
            }

            // Переключиться на следующий слот
            MoveToNextSlot();

            return true;
        }

        /// <summary>
        /// Найти следующий слот с заклинанием (пропуская бафы)
        /// </summary>
        private WandSlot FindNextSpellSlot()
        {
            if (slots.Count == 0) return WandSlot.Empty;

            int startIndex = currentSlotIndex;
            int attempts = 0;

            while (attempts < slots.Count)
            {
                WandSlot slot = slots[currentSlotIndex];

                if (slot.IsSpell)
                {
                    return slot;
                }

                // Переключиться на следующий слот
                currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
                attempts++;
            }

            return WandSlot.Empty;
        }

        /// <summary>
        /// Построить базовый CastContext БЕЗ модификаторов
        /// (На Этапе 8 здесь будет применение бафов)
        /// </summary>
        private CastContext BuildBasicCastContext(SpellSO spell)
        {
            // Builder принимает ICaster (используем caster или wrapper)
            var builder = new CastContext.Builder(caster, spell);

            // На Этапе 7 просто используем базовые параметры
            // (модификаторы будут добавлены на Этапе 8)

            return builder.Build();
        }

        /// <summary>
        /// Переключиться на следующий слот
        /// </summary>
        private void MoveToNextSlot()
        {
            if (slots.Count == 0) return;

            currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
            OnSlotChanged?.Invoke(currentSlotIndex);
        }

        /// <summary>
        /// Добавить слот
        /// </summary>
        public bool AddSlot(WandSlot slot)
        {
            if (slots.Count >= maxSlots)
            {
                Debug.LogWarning("[Wand] Cannot add slot: max slots reached!");
                return false;
            }

            slots.Add(slot);
            OnSlotAdded?.Invoke(slot);
            return true;
        }

        /// <summary>
        /// Удалить слот по индексу
        /// </summary>
        public bool RemoveSlot(int index)
        {
            if (index < 0 || index >= slots.Count)
            {
                return false;
            }

            slots.RemoveAt(index);

            // Корректировка текущего индекса
            if (currentSlotIndex >= slots.Count)
            {
                currentSlotIndex = Mathf.Max(0, slots.Count - 1);
            }

            OnSlotRemoved?.Invoke(index);
            return true;
        }

        /// <summary>
        /// Получить слот по индексу
        /// </summary>
        public WandSlot GetSlot(int index)
        {
            if (index >= 0 && index < slots.Count)
            {
                return slots[index];
            }
            return WandSlot.Empty;
        }

        /// <summary>
        /// Установить слот по индексу
        /// </summary>
        public void SetSlot(int index, WandSlot slot)
        {
            if (index >= 0 && index < slots.Count)
            {
                slots[index] = slot;
            }
        }

        /// <summary>
        /// Поменять местами два слота
        /// </summary>
        public void SwapSlots(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= slots.Count) return;
            if (indexB < 0 || indexB >= slots.Count) return;

            var temp = slots[indexA];
            slots[indexA] = slots[indexB];
            slots[indexB] = temp;
        }

        /// <summary>
        /// Очистить все слоты
        /// </summary>
        public void ClearAllSlots()
        {
            slots.Clear();
            currentSlotIndex = 0;
        }

        /// <summary>
        /// Получить все слоты
        /// </summary>
        public List<WandSlot> GetAllSlots()
        {
            return new List<WandSlot>(slots);
        }

        // Debug методы
        [ContextMenu("Debug: Print All Slots")]
        private void DebugPrintSlots()
        {
            Debug.Log($"[Wand] Total slots: {slots.Count}/{maxSlots}");
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                string marker = i == currentSlotIndex ? " <-- CURRENT" : "";
                Debug.Log($"  [{i}] {slot.Type}: {slot.GetDisplayName()}{marker}");
            }
        }

        [ContextMenu("Debug: Add Test Spell")]
        private void DebugAddTestSpell()
        {
            // Попытаться найти любое SpellSO в проекте
            SpellSO testSpell = Resources.Load<SpellSO>("Spells/MagicMissile");
            if (testSpell != null)
            {
                AddSlot(WandSlot.FromSpell(testSpell));
                Debug.Log("[Wand] Added test spell!");
            }
            else
            {
                Debug.LogWarning("[Wand] No test spell found!");
            }
        }
    }

    /// <summary>
    /// Временный wrapper для работы Wand без ICaster на Этапе 7
    /// Предоставляет минимальную реализацию ICaster
    /// </summary>
    internal class WandCasterWrapper : ICaster
    {
        private Transform wandTransform;
        private Transform muzzle;

        public WandCasterWrapper(Transform wandTransform)
        {
            this.wandTransform = wandTransform;
            // Попытаться найти дочерний объект "Muzzle", иначе использовать сам transform
            var muzzleChild = wandTransform.Find("Muzzle");
            this.muzzle = muzzleChild != null ? muzzleChild : wandTransform;
        }

        public Transform Muzzle => muzzle;

        public Vector2 AimDirection
        {
            get
            {
                // Направление в сторону мыши (новая Input System)
                if (Mouse.current == null || Camera.main == null)
                {
                    return Vector2.right;
                }

                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane));
                mouseWorldPos.z = 0f;
                Vector2 direction = ((Vector2)mouseWorldPos - (Vector2)wandTransform.position).normalized;
                return direction;
            }
        }

        public bool TrySpendMana(float amount)
        {
            // На Этапе 7 мана не тратится
            return true;
        }

        public void OnSpellCasted(CastContext context)
        {
            // Ничего не делаем
        }
    }
}
