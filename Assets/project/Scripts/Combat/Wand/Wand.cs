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
        /// ЭТАП 8: Теперь можно загружать не только заклинания из baseSpells,
        /// но и вручную добавлять бафы через AddSlot
        /// </summary>
        private void LoadFromWandData()
        {
            slots.Clear();

            // Добавить базовые заклинания из WandSO
            if (wandData.baseSpells != null)
            {
                foreach (var spell in wandData.baseSpells)
                {
                    if (spell != null)
                    {
                        AddSlot(WandSlot.FromSpell(spell));
                    }
                }
            }

            maxSlots = wandData.maxSpellSlots;

            // Debug информация о пассивных баффах
            if (wandData.passiveBuffs != null && wandData.passiveBuffs.Count > 0)
            {
                Debug.Log($"[Wand] Loaded with {wandData.passiveBuffs.Count} passive buffs:");
                foreach (var buff in wandData.passiveBuffs)
                {
                    if (buff != null)
                    {
                        Debug.Log($"  • {buff.DisplayName}");
                    }
                }
            }
        }

        /// <summary>
        /// Попытка каста текущего слота
        /// ЭТАП 8: С применением модификаторов от баффов справа
        /// </summary>
        public bool TryCast()
        {
            if (!IsReady)
            {
                Debug.Log("[Wand] Not ready to cast (cooldown)");
                return false;
            }

            // Найти следующее заклинание для каста и запомнить его индекс
            int spellSlotIndex = FindNextSpellSlotIndex();

            if (spellSlotIndex < 0 || spellSlotIndex >= slots.Count)
            {
                Debug.Log("[Wand] No spell to cast!");
                return false;
            }

            WandSlot spellSlot = slots[spellSlotIndex];

            if (spellSlot.Spell == null)
            {
                Debug.LogError("[Wand] SpellSO is null on the selected slot");
                return false;
            }

            // Построить CastContext С МОДИФИКАТОРАМИ (Этап 8)
            CastContext context = BuildCastContextWithModifiers(spellSlot.Spell, spellSlotIndex);

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
        /// Найти индекс следующего слота с заклинанием (пропуская бафы)
        /// </summary>
        private int FindNextSpellSlotIndex()
        {
            if (slots.Count == 0) return -1;

            int startIndex = currentSlotIndex;
            int attempts = 0;

            while (attempts < slots.Count)
            {
                WandSlot slot = slots[currentSlotIndex];

                if (slot.IsSpell)
                {
                    return currentSlotIndex;
                }

                // Переключиться на следующий слот
                currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
                attempts++;
            }

            return -1;
        }

        /// <summary>
        /// Построить CastContext с применением модификаторов
        /// ЭТАП 8: Применение бафов справа от заклинания + пассивные баффы посоха
        /// </summary>
        private CastContext BuildCastContextWithModifiers(SpellSO spell, int spellSlotIndex)
        {
            var builder = new CastContext.Builder(caster, spell);

            // 1. Применить пассивные баффы посоха (если есть WandSO)
            if (wandData != null && wandData.passiveBuffs != null)
            {
                foreach (var buff in wandData.passiveBuffs)
                {
                    if (buff != null && buff.CanAffectSpell(spell))
                    {
                        ApplyBuffToBuilder(builder, buff);
                    }
                }

                // Применить глобальные множители посоха
                builder.ApplyDamageMultiplier(wandData.damageMultiplier);
                builder.ApplyManaCostMultiplier(wandData.manaCostMultiplier);
                builder.ApplyCooldownMultiplier(wandData.cooldownMultiplier);
            }

            // 2. Применить баффы справа от заклинания (rightCumulative алгоритм)
            // Все баффы СПРАВА от текущего слота модифицируют это заклинание
            for (int i = spellSlotIndex + 1; i < slots.Count; i++)
            {
                WandSlot slot = slots[i];
                if (slot.IsBuff && slot.Buff != null)
                {
                    // Проверить фильтр по тегам
                    if (slot.Buff.CanAffectSpell(spell))
                    {
                        ApplyBuffToBuilder(builder, slot.Buff);
                    }
                }
            }

            return builder.Build();
        }

        /// <summary>
        /// Применить баф к Builder с учётом режима стакания
        /// </summary>
        private void ApplyBuffToBuilder(CastContext.Builder builder, BuffSO buff)
        {
            // Мультипликативные модификаторы
            builder.ApplyDamageMultiplier(buff.DamageMultiplier);
            builder.ApplyManaCostMultiplier(buff.ManaCostMultiplier);
            builder.ApplyCooldownMultiplier(buff.CooldownMultiplier);
            builder.ApplySpeedMultiplier(buff.ProjectileSpeedMultiplier);

            // Аддитивные модификаторы
            builder.AddPierce(buff.AddPierce);
            builder.AddCritChance(buff.AddCritChance);

            // Крит множитель (берём максимальный)
            if (buff.CritMultiplier > 1f)
            {
                builder.SetCritMultiplier(buff.CritMultiplier);
            }
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
        /// Добавить слот из ScriptableObject (для RewardSystem)
        /// </summary>
        public bool AddSlot(ScriptableObject data, SlotType type)
        {
            if (data == null)
            {
                Debug.LogWarning("[Wand] Cannot add slot: data is null!");
                return false;
            }

            WandSlot newSlot = WandSlot.Empty;

            if (type == SlotType.Spell)
            {
                SpellSO spell = data as SpellSO;
                if (spell != null)
                {
                    newSlot = WandSlot.FromSpell(spell);
                }
                else
                {
                    Debug.LogError("[Wand] Data is not a SpellSO!");
                    return false;
                }
            }
            else if (type == SlotType.Buff)
            {
                BuffSO buff = data as BuffSO;
                if (buff != null)
                {
                    newSlot = WandSlot.FromBuff(buff);
                }
                else
                {
                    Debug.LogError("[Wand] Data is not a BuffSO!");
                    return false;
                }
            }

            return AddSlot(newSlot);
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

        [ContextMenu("Debug: Test Modifiers")]
        private void DebugTestModifiers()
        {
            if (slots.Count == 0)
            {
                Debug.LogWarning("[Wand] No slots to test!");
                return;
            }

            Debug.Log("=== MODIFIER TEST ===");
            
            // Вывести информацию о пассивных баффах посоха
            if (wandData != null && wandData.passiveBuffs.Count > 0)
            {
                Debug.Log($"Wand Passive Buffs ({wandData.passiveBuffs.Count}):");
                foreach (var buff in wandData.passiveBuffs)
                {
                    if (buff != null)
                    {
                        Debug.Log($"  • {buff.DisplayName}: DMG x{buff.DamageMultiplier}, Mana x{buff.ManaCostMultiplier}, Speed x{buff.ProjectileSpeedMultiplier}, Pierce +{buff.AddPierce}");
                    }
                }
            }

            // Для каждого заклинания показать, какие баффы на него влияют
            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                if (slot.IsSpell && slot.Spell != null)
                {
                    Debug.Log($"\n[{i}] SPELL: {slot.Spell.DisplayName}");
                    Debug.Log($"  Base: DMG={slot.Spell.BaseDamage}, Mana={slot.Spell.BaseManaCost}, Speed={slot.Spell.ProjectileSpeed}, Pierce={slot.Spell.Pierce}");

                    // Показать баффы справа
                    List<string> affectingBuffs = new List<string>();
                    for (int j = i + 1; j < slots.Count; j++)
                    {
                        var buffSlot = slots[j];
                        if (buffSlot.IsBuff && buffSlot.Buff != null)
                        {
                            if (buffSlot.Buff.CanAffectSpell(slot.Spell))
                            {
                                affectingBuffs.Add($"[{j}] {buffSlot.Buff.DisplayName}");
                            }
                        }
                    }

                    if (affectingBuffs.Count > 0)
                    {
                        Debug.Log($"  Affected by buffs: {string.Join(", ", affectingBuffs)}");
                    }
                    else
                    {
                        Debug.Log($"  No buffs affecting this spell");
                    }

                    // Симулировать каст и показать финальные параметры
                    if (caster != null)
                    {
                        var context = BuildCastContextWithModifiers(slot.Spell, i);
                        Debug.Log($"  FINAL: DMG={context.Damage:F1}, Mana={context.ManaCost:F1}, Speed={context.ProjectileSpeed:F1}, Pierce={context.Pierce}, CD={context.Cooldown:F2}s");
                    }
                }
            }
            
            Debug.Log("=== END TEST ===");
        }

        [ContextMenu("Debug: Add Test Spell")]
        private void DebugAddTestSpell()
        {
            #if UNITY_EDITOR
            // В режиме редактора можем использовать AssetDatabase
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SpellSO", new[] { "Assets/project/ScriptableObjects/Spells" });
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                SpellSO testSpell = UnityEditor.AssetDatabase.LoadAssetAtPath<SpellSO>(path);
                if (testSpell != null)
                {
                    AddSlot(WandSlot.FromSpell(testSpell));
                    Debug.Log($"[Wand] Added test spell: {testSpell.DisplayName}");
                    return;
                }
            }
            Debug.LogWarning("[Wand] No SpellSO found in project! Create one in ScriptableObjects/Spells/");
            #else
            Debug.LogWarning("[Wand] This debug method only works in Editor mode!");
            #endif
        }

        [ContextMenu("Debug: Add Test Buff")]
        private void DebugAddTestBuff()
        {
            #if UNITY_EDITOR
            // В режиме редактора можем использовать AssetDatabase
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BuffSO", new[] { "Assets/project/ScriptableObjects/Buffs" });
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                BuffSO testBuff = UnityEditor.AssetDatabase.LoadAssetAtPath<BuffSO>(path);
                if (testBuff != null)
                {
                    AddSlot(WandSlot.FromBuff(testBuff));
                    Debug.Log($"[Wand] Added test buff: {testBuff.DisplayName}");
                    return;
                }
            }
            Debug.LogWarning("[Wand] No BuffSO found in project! Create one in ScriptableObjects/Buffs/");
            #else
            Debug.LogWarning("[Wand] This debug method only works in Editor mode!");
            #endif
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
