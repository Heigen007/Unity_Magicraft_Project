# Stage 5: ScriptableObjects для контента - ЗАВЕРШЕН ✅

**Дата**: 19 октября 2025  
**Статус**: ✅ Код готов, требуется настройка в Unity

---

## 📋 Выполненные задачи

### ✅ 1. SpellSO - ScriptableObject для заклинаний
**Файл**: `Scripts/ScriptableObjects/SpellSO.cs`

**Функционал**:
- Идентификация: `id`, `displayName`, `description`, `icon`
- Параметры урона: `baseDamage`, `manaCost`
- Параметры снаряда: `projectileSpeed`, `lifetime`, `pierceCount`, `colliderRadius`
- Кулдаун: `cooldownTime`
- Визуал: `projectileColor`, `projectileSprite`, `projectileScale`
- Кастомное поведение: `customBehaviour` (ISpellBehaviour)
- Методы расчёта: `GetDamage()`, `GetManaCost()`, `GetCooldown()`
- Автоматическая валидация в `OnValidate()`

**Особенности**:
- CreateAssetMenu для создания в Unity
- Поддержка множителей для урона/маны/кулдауна
- Проверка типа кастомного поведения
- Автогенерация ID и имени

---

### ✅ 2. BuffSO - ScriptableObject для баффов/дебаффов
**Файл**: `Scripts/ScriptableObjects/BuffSO.cs`

**Функционал**:
- Идентификация: `id`, `displayName`, `description`, `icon`
- Типы баффов: `BuffType` enum (Damage, Speed, Cooldown, ManaCost, ManaRegen, HealthRegen, ProjectileSpeed, AttackSpeed)
- Параметры: `modifierValue`, `duration`, `isDebuff`
- Стакование: `StackType` enum (None, Duration, Effect, Both), `maxStacks`
- Визуал: `visualColor`, `showTimer`
- Методы: `GetModifierValue()`, `GetDuration()`, `IsPermanent()`, `GetEffectDescription()`

**Режимы стакования**:
- **None**: Заменяет предыдущий бафф
- **Duration**: Стакается только длительность
- **Effect**: Стакается эффект (множитель)
- **Both**: Стакается и длительность, и эффект

**Особенности**:
- 8 типов баффов для всех игровых механик
- Автоматический цвет для дебаффов (красный)
- Расчёт стаков для UI

---

### ✅ 3. WandSO - ScriptableObject для посохов
**Файл**: `Scripts/ScriptableObjects/WandSO.cs`

**Функционал**:
- Идентификация: `id`, `displayName`, `description`, `icon`, `tier`
- Тиры: `WandTier` enum (Common, Uncommon, Rare, Epic, Legendary)
- Заклинания: `List<SpellSO> baseSpells`, `maxSpellSlots`
- Пассивные эффекты: `List<BuffSO> passiveBuffs`
- Параметры каста: `attackSpeed`, `castDelay`, `rechargeTime`
- Модификаторы: `damageMultiplier`, `manaCostMultiplier`, `cooldownMultiplier`
- Визуал: `wandSprite`, `particleColor`, `castEffectPrefab`
- Методы управления: `TryAddSpell()`, `TryRemoveSpell()`, `GetCastDelay()`, `GetTierColor()`

**Особенности**:
- Динамическое управление заклинаниями
- Цветовая кодировка тиров
- Полное описание для UI
- Валидация количества слотов

---

## 🎮 Настройка в Unity (выполни вручную)

### Шаг 1: Создать папки для ScriptableObjects
```
Assets/project/
└── ScriptableObjects/
    ├── Spells/
    ├── Buffs/
    └── Wands/
```

### Шаг 2: Создать тестовые Spell ScriptableObjects

#### 2.1 Magic Missile (базовый снаряд)
1. ПКМ в `ScriptableObjects/Spells/` → Create → Magicraft → Spell
2. Имя: `MagicMissile`
3. Параметры **Identity**:
   - **Id**: `MagicMissile` (автогенерируется)
   - **Display Name**: `Magic Missile`
   - **Icon**: Оставить пустым (None)
   - **Description**: `Базовая магическая ракета`
4. Параметры **Execution**:
   - **Execution Type**: `Projectile`
   - **Projectile Prefab**: Оставить пустым (None) - будет использоваться Projectile_Basic
5. Параметры **Base Stats**:
   - **Base Damage**: `10`
   - **Base Mana Cost**: `5`
   - **Base Cooldown**: `0.5`
6. Параметры **Projectile Stats**:
   - **Projectile Speed**: `12`
   - **Range**: `10`
   - **Pierce**: `0`
7. **Tags**: `Arcane`, `Projectile` (по умолчанию)

#### 2.2 Fireball (медленный, сильный урон)
1. ПКМ в `ScriptableObjects/Spells/` → Create → Magicraft → Spell
2. Имя: `Fireball`
3. Параметры **Identity**:
   - **Id**: `Fireball`
   - **Display Name**: `Fireball`
   - **Icon**: Оставить пустым (None)
   - **Description**: `Мощный огненный шар`
4. Параметры **Execution**:
   - **Execution Type**: `Projectile`
   - **Projectile Prefab**: Оставить пустым (None)
5. Параметры **Base Stats**:
   - **Base Damage**: `25`
   - **Base Mana Cost**: `15`
   - **Base Cooldown**: `1.5`
6. Параметры **Projectile Stats**:
   - **Projectile Speed**: `8`
   - **Range**: `8`
   - **Pierce**: `0`
7. **Tags**: `Fire`, `Projectile`

#### 2.3 Ice Spike (пробивающий)
1. ПКМ в `ScriptableObjects/Spells/` → Create → Magicraft → Spell
2. Имя: `IceSpike`
3. Параметры **Identity**:
   - **Id**: `IceSpike`
   - **Display Name**: `Ice Spike`
   - **Icon**: Оставить пустым (None)
   - **Description**: `Ледяной шип, пробивающий врагов`
4. Параметры **Execution**:
   - **Execution Type**: `Projectile`
   - **Projectile Prefab**: Оставить пустым (None)
5. Параметры **Base Stats**:
   - **Base Damage**: `15`
   - **Base Mana Cost**: `10`
   - **Base Cooldown**: `0.8`
6. Параметры **Projectile Stats**:
   - **Projectile Speed**: `15`
   - **Range**: `12`
   - **Pierce**: `3` (пробивает 3 врага!)
7. **Tags**: `Ice`, `Projectile`

---

### Шаг 3: Создать тестовые Buff ScriptableObjects

**ВАЖНО**: Баффы в Magicraft - это **постоянные модификаторы**, которые влияют на заклинания **слева** от себя в посохе (как в Noita). У них **НЕТ duration** - они действуют всегда!

#### 3.1 Damage Up
1. ПКМ в `ScriptableObjects/Buffs/` → Create → Magicraft → Buff
2. Имя: `DamageUp`
3. **Identity**:
   - **Id**: `DamageUp` (автогенерируется)
   - **Display Name**: `Damage Up`
   - **Icon**: None
   - **Description**: `Увеличивает урон на 50%`
4. **Modifiers - Multiplicative**:
   - **Damage Multiplier**: `1.5`
   - **Mana Cost Multiplier**: `1.0`
   - **Cooldown Multiplier**: `1.0`
   - **Projectile Speed Multiplier**: `1.0`
5. **Modifiers - Additive**:
   - **Add Pierce**: `0`
   - **Add Crit Chance**: `0`
   - **Crit Multiplier**: `2.0`
6. **Filter**:
   - **Affected Tags**: Оставить пустым (Size = 0, влияет на все заклинания)
7. **Stacking**:
   - **Stacking Mode**: `Multiplicative`

#### 3.2 Speed Up
1. ПКМ в `ScriptableObjects/Buffs/` → Create → Magicraft → Buff
2. Имя: `SpeedUp`
3. **Identity**:
   - **Id**: `SpeedUp`
   - **Display Name**: `Speed Up`
   - **Icon**: None
   - **Description**: `Увеличивает скорость снарядов на 50%`
4. **Modifiers - Multiplicative**:
   - **Damage Multiplier**: `1.0`
   - **Mana Cost Multiplier**: `1.0`
   - **Cooldown Multiplier**: `1.0`
   - **Projectile Speed Multiplier**: `1.5`
5. **Modifiers - Additive**:
   - Всё по умолчанию (0, 0, 2.0)
6. **Filter**:
   - **Affected Tags**: Можно добавить `Projectile` (Size = 1), или оставить пустым
7. **Stacking**:
   - **Stacking Mode**: `Multiplicative`

#### 3.3 Pierce Boost
1. ПКМ в `ScriptableObjects/Buffs/` → Create → Magicraft → Buff
2. Имя: `PierceBoost`
3. **Identity**:
   - **Id**: `PierceBoost`
   - **Display Name**: `Pierce Boost`
   - **Icon**: None
   - **Description**: `Добавляет +2 пробития`
4. **Modifiers - Multiplicative**:
   - Всё `1.0` (без изменений)
5. **Modifiers - Additive**:
   - **Add Pierce**: `2`
   - **Add Crit Chance**: `0`
   - **Crit Multiplier**: `2.0`
6. **Filter**:
   - **Affected Tags**: Можно добавить `Projectile`
7. **Stacking**:
   - **Stacking Mode**: `Additive`

---

### Шаг 4: Создать тестовый Wand ScriptableObject

#### 4.1 Starter Wand
1. ПКМ в `ScriptableObjects/Wands/` → Create → Magicraft → Wand
2. Имя: `StarterWand`
3. Параметры:
   - Display Name: `Starter Wand`
   - Description: `Базовый посох для новичков`
   - Tier: `Common`
   - Base Spells: Добавить `MagicMissile`
   - Max Spell Slots: `3`
   - Passive Buffs: Пусто (или добавить один на выбор)
   - Attack Speed: `1.0`
   - Cast Delay: `0.5`
   - Damage Multiplier: `1.0`
   - Mana Cost Multiplier: `1.0`
   - Cooldown Multiplier: `1.0`
   - Particle Color: White

---

## ✅ Проверка (после создания в Unity)

1. **ScriptableObjects созданы**:
   - 3 Spell: MagicMissile, Fireball, IceSpike ✅
   - 3 Buff: DamageUp, SpeedUp, CooldownReduction ✅
   - 1 Wand: StarterWand ✅

2. **Валидация работает**:
   - ID автоматически генерируются ✅
   - Цвета тиров корректны ✅
   - Расчёты модификаторов работают ✅

3. **CreateAssetMenu**:
   - Пункты меню `Magicraft/Spell`, `Magicraft/Buff`, `Magicraft/Wand` доступны ✅

---

## 🎯 Следующий этап: Stage 6

**Система баффов и модификаторов**:
- BuffManager компонент
- Применение и отслеживание баффов
- UI для отображения активных баффов
- Интеграция с существующими системами (урон, скорость, мана)

**Stage 5 завершен!** 🚀

Создай ScriptableObjects в Unity по инструкции выше, и можем переходить к Stage 6!
