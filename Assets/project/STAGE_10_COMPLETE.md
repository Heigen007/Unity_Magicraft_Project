# Stage 10: Reward System - Полное руководство

## 📋 Обзор

На этом этапе реализована **система наград** за убийство врагов:
- Каждые **10 убийств** → игра ставится на паузу
- Игрок выбирает **1 из 3** карточек (Spell или Buff)
- Выбранная награда **добавляется в посох**
- Игра продолжается

---

## 🗂 Файлы

### Scripts/Rewards/
- **RewardOption.cs** - структура награды (Spell/Buff)
- **RewardSystem.cs** - логика наград (счётчик убийств, генерация опций)

### Scripts/UI/
- **RewardCard.cs** - компонент карточки награды
- **RewardChoiceUI.cs** - панель выбора (3 карточки)
- **KillCounterUI.cs** - счётчик убийств в HUD

### Изменения в существующих файлах:
- `Scripts/Combat/Wand/Wand.cs` - добавлен метод `AddSlot(ScriptableObject, SlotType)`
- `Scripts/Combat/Enemy/EnemyController.cs` - добавлен static event `OnAnyEnemyKilled`

---

## 🎨 Настройка UI в Unity

### 1. Создать префаб RewardCard

1. **Hierarchy** → ПКМ → **UI** → **Panel** → назвать `RewardCard`

2. **Настроить RewardCard**:
   - Rect Transform:
     - Width: `250`, Height: `350`
   - Image (рамка):
     - Color: белый (цвет будет меняться кодом)
     - Sprite: `UISprite` (по умолчанию)

3. **Добавить дочерние элементы**:

   **a) Border (рамка цветная)**:
   - ПКМ на RewardCard → UI → Image → назвать `Border`
   - Rect Transform: Anchors: **Stretch** (заполняет родителя)
   - Offsets: Left=`0`, Right=`0`, Top=`0`, Bottom=`0`
   - Image: Color будет меняться кодом (синий для Spell, оранжевый для Buff)

   **b) Icon**:
   - ПКМ на RewardCard → UI → Image → назвать `Icon`
   - Rect Transform:
     - Anchors: Top Center
     - Width: `120`, Height: `120`
     - Pos Y: `-80`
   - Image: Sprite - оставить пустым (будет заполняться кодом)

   **c) TypeText**:
   - ПКМ на RewardCard → UI → Text - TextMeshPro → назвать `TypeText`
   - Rect Transform:
     - Anchors: Top Center
     - Width: `200`, Height: `40`
     - Pos Y: `-20`
   - TextMeshPro:
     - Text: `SPELL`
     - Font Size: `24`
     - Alignment: Center
     - Color: белый

   **d) NameText**:
   - ПКМ на RewardCard → UI → Text - TextMeshPro → назвать `NameText`
   - Rect Transform:
     - Anchors: Center
     - Width: `220`, Height: `50`
     - Pos Y: `20`
   - TextMeshPro:
     - Text: `Reward Name`
     - Font Size: `20`
     - Alignment: Center
     - Color: белый

   **e) DescriptionText**:
   - ПКМ на RewardCard → UI → Text - TextMeshPro → назвать `DescriptionText`
   - Rect Transform:
     - Anchors: Bottom Center
     - Width: `220`, Height: `100`
     - Pos Y: `90`
   - TextMeshPro:
     - Text: `Description here`
     - Font Size: `14`
     - Alignment: Center + Top
     - Color: светло-серый

   **f) SelectButton**:
   - ПКМ на RewardCard → UI → Button - TextMeshPro → назвать `SelectButton`
   - Rect Transform:
     - Anchors: Bottom Center
     - Width: `200`, Height: `50`
     - Pos Y: `30`
   - Button: Normal Color - зелёный, Hover - светло-зелёный
   - Text (дочерний):
     - Text: `SELECT`
     - Font Size: `18`
     - Alignment: Center
     - Color: белый

4. **Добавить RewardCard компонент**:
   - Add Component → **Reward Card**
   - Настроить ссылки:
     - Icon Image: перетащить `Icon`
     - Name Text: перетащить `NameText`
     - Description Text: перетащить `DescriptionText`
     - Type Text: перетащить `TypeText`
     - Select Button: перетащить `SelectButton`
     - Border Image: перетащить `Border`
   - Colors:
     - Spell Color: `RGB(76, 128, 255)` (синий)
     - Buff Color: `RGB(255, 128, 76)` (оранжевый)

5. **Сохранить как префаб**:
   - Перетащить `RewardCard` из Hierarchy в `Prefabs/` → переименовать в `RewardCard.prefab`
   - Удалить из Hierarchy

---

### 2. Создать панель RewardChoiceUI

1. **Hierarchy** → найти **HUDCanvas** (или Canvas)

2. **ПКМ на Canvas** → UI → Panel → назвать `RewardChoicePanel`

3. **Настроить RewardChoicePanel**:
   - Rect Transform:
     - Anchors: **Stretch** (заполняет весь экран)
     - Offsets: Left=`0`, Right=`0`, Top=`0`, Bottom=`0`
   - Image:
     - Color: `RGB(0, 0, 0, 200)` (полупрозрачный чёрный фон)

4. **Создать контейнер для карточек**:
   - ПКМ на RewardChoicePanel → UI → Panel → назвать `CardsContainer`
   - Rect Transform:
     - Anchors: Center
     - Width: `800`, Height: `400`
   - Image: отключить (убрать галочку Enabled)
   - Add Component → **Horizontal Layout Group**:
     - Child Alignment: Middle Center
     - Spacing: `20`
     - Child Force Expand: Width ✓, Height ✓

5. **Добавить заголовок**:
   - ПКМ на RewardChoicePanel → UI → Text - TextMeshPro → назвать `TitleText`
   - Rect Transform:
     - Anchors: Top Center
     - Width: `600`, Height: `80`
     - Pos Y: `-50`
   - TextMeshPro:
     - Text: `CHOOSE YOUR REWARD`
     - Font Size: `36`
     - Alignment: Center
     - Color: белый

6. **Добавить RewardChoiceUI компонент**:
   - Выбрать `RewardChoicePanel`
   - Add Component → **Reward Choice UI**
   - Настроить:
     - Panel: перетащить сам `RewardChoicePanel`
     - Cards Container: перетащить `CardsContainer`
     - Card Prefab: перетащить `RewardCard.prefab`
     - Pause Game On Show: ✓

---

### 3. Добавить Kill Counter в HUD

1. **Hierarchy** → найти **HUDCanvas**

2. **ПКМ на HUDCanvas** → UI → Panel → назвать `KillCounterPanel`

3. **Настроить KillCounterPanel**:
   - Rect Transform:
     - Anchors: Top Right
     - Pivot: `1, 1`
     - Pos X: `-20`, Pos Y: `-20`
     - Width: `200`, Height: `80`
   - Image:
     - Color: `RGB(0, 0, 0, 150)` (полупрозрачный чёрный)

4. **Добавить тексты**:

   **a) CounterText**:
   - ПКМ на KillCounterPanel → UI → Text - TextMeshPro → назвать `CounterText`
   - Rect Transform:
     - Anchors: Stretch
     - Offsets: Left=`10`, Right=`10`, Top=`10`, Bottom=`45`
   - TextMeshPro:
     - Text: `Kills: 0`
     - Font Size: `20`
     - Alignment: Center
     - Color: белый

   **b) ProgressText**:
   - ПКМ на KillCounterPanel → UI → Text - TextMeshPro → назвать `ProgressText`
   - Rect Transform:
     - Anchors: Stretch
     - Offsets: Left=`10`, Right=`10`, Top=`45`, Bottom=`10`
   - TextMeshPro:
     - Text: `Next reward: 10 kills`
     - Font Size: `14`
     - Alignment: Center
     - Color: светло-серый

5. **Добавить KillCounterUI компонент**:
   - Выбрать `KillCounterPanel`
   - Add Component → **Kill Counter UI**
   - Настроить:
     - Counter Text: перетащить `CounterText`
     - Progress Text: перетащить `ProgressText`
     - Reward System: оставить пустым (найдётся автоматически)

---

### 4. Настроить RewardSystem

1. **Hierarchy** → найти **Player** (или создать пустой GameObject `GameSystems`)

2. **Создать GameObject**:
   - ПКМ в Hierarchy → Create Empty → назвать `RewardSystem`

3. **Добавить компонент**:
   - Add Component → **Reward System**
   - Настроить:
     - Kills Per Reward: `10`
     - Options Count: `3`
     - Reward Choice UI: перетащить `RewardChoicePanel`
     - Player Wand: перетащить `Player` (автоматически найдёт Wand)
     - Available Spells:
       - Size: `3`
       - Element 0: `Fireball.asset`
       - Element 1: `MagicMissile.asset`
       - Element 2: `Ice Spike.asset`
     - Available Buffs:
       - Size: `3`
       - Element 0: `DamageUp.asset`
       - Element 1: `SpeedUp.asset`
       - Element 2: `Pierce Boost.asset`

---

## 🧪 Тестирование

### Тест 1: Debug - мгновенная награда

1. **Запустить игру** (Play Mode)

2. **Player** → **Wand** → **ПКМ** → **Debug: Add 5 Kills** (дважды)

3. **Проверить**:
   - После 10 убийств появляется панель с **3 карточками**
   - Игра **ставится на паузу** (Time.timeScale = 0)
   - Карточки показывают:
     - Иконки (если есть в SO)
     - Названия
     - Описания
     - Тип (SPELL синий, BUFF оранжевый)

✅ **Результат**: Панель наград появляется корректно

---

### Тест 2: Выбор награды

1. **В панели наград кликнуть** на любую карточку

2. **Проверить Console**:
   ```
   [RewardChoiceUI] Reward selected: Fireball
   [RewardSystem] Player chose: Fireball (Spell)
   ```

3. **Проверить Wand**:
   - Player → Wand → Slots → Size увеличился на 1
   - Новый слот добавлен с выбранным SO

4. **Проверить UI**:
   - Панель наград **скрывается**
   - Игра **продолжается** (Time.timeScale = 1)
   - WandEditorUI **обновляется** автоматически

✅ **Результат**: Награда добавляется в посох, игра продолжается

---

### Тест 3: Счётчик убийств

1. **Запустить игру**

2. **Убить 5 врагов** (стрельбой)

3. **Проверить HUD**:
   - Kill Counter показывает: `Kills: 5`
   - Progress: `Next reward: 5 kills`

4. **Убить ещё 5 врагов**

5. **Проверить**:
   - После 10 убийств появляется панель наград
   - Счётчик: `Kills: 10`
   - Progress: `Next reward: 10 kills` (следующая на 20)

✅ **Результат**: Счётчик работает, награды появляются каждые 10 убийств

---

### Тест 4: Несколько наград подряд

1. **Запустить игру**

2. **RewardSystem** → **ПКМ** → **Debug: Add 5 Kills** × 4 раза (= 20 убийств)

3. **Проверить**:
   - Первая награда на 10 убийствах
   - Вторая награда на 20 убийствах
   - Каждый раз игра паузится
   - Посох увеличивается после каждого выбора

✅ **Результат**: Система наград работает многократно

---

## 🔧 Как это работает

### Архитектура

```
EnemyController (static event OnAnyEnemyKilled)
         ↓
RewardSystem (слушает, считает убийства)
         ↓
RewardChoiceUI (показывает 3 карточки)
         ↓
RewardCard × 3 (визуал + кнопка)
         ↓
Wand.AddSlot() (добавляет выбранное)
         ↓
WandEditorUI.RefreshUI() (автообновление через OnSlotAdded)
```

### Алгоритм наград

1. **Враг умирает** → `EnemyController.OnDeath(killer)`
2. **Static event** → `OnAnyEnemyKilled?.Invoke(killer)`
3. **RewardSystem** слушает → `OnEnemyKilled(killer)`
4. **Проверка убийц** → `if (killer.CompareTag("Player"))`
5. **Счётчик** → `totalKills++`
6. **Каждые 10** → `ShowRewardChoice()`
7. **Генерация опций** → `GenerateRewardOptions(3)`
8. **Показ UI** → `RewardChoiceUI.ShowRewards(options)`
9. **Пауза** → `Time.timeScale = 0`
10. **Клик** → `OnCardSelected(reward)`
11. **Добавление** → `Wand.AddSlot(reward.Data, reward.type)`
12. **Снятие паузы** → `Time.timeScale = 1`

---

## 💡 Детали реализации

### RewardOption

- **Enum** `RewardType` (Spell / Buff)
- **Factory методы**: `CreateSpell()`, `CreateBuff()`
- **Рефлексия**: получает `description` и `icon` из SO

### RewardSystem

- **Static event subscription**: не нужно искать врагов вручную
- **Автопоиск Wand**: `FindGameObjectWithTag("Player").GetComponent<Wand>()`
- **Debug методы**: `Debug: Grant Reward Now`, `Debug: Add 5 Kills`

### RewardCard

- **Динамические цвета**: синий для Spell, оранжевый для Buff
- **Рефлексия**: читает `icon` из SO через `GetType().GetField("icon")`
- **Event-driven**: `OnCardSelected` → `RewardChoiceUI`

### RewardChoiceUI

- **Пауза**: `Time.timeScale = 0` при показе
- **Динамические карточки**: создаёт из префаба через `Instantiate()`
- **Cleanup**: удаляет карточки после выбора

---

## ⚠️ Важные моменты

### 1. Static Event Pattern

```csharp
// EnemyController.cs
public static event System.Action<GameObject> OnAnyEnemyKilled;

// В OnDeath():
OnAnyEnemyKilled?.Invoke(killer);

// RewardSystem.cs
void Start() {
    EnemyController.OnAnyEnemyKilled += OnEnemyKilled;
}

void OnDestroy() {
    EnemyController.OnAnyEnemyKilled -= OnEnemyKilled;
}
```

**Преимущество**: не нужно искать всех врагов и подписываться вручную!

### 2. Time.timeScale и Pause

```csharp
Time.timeScale = 0f; // Пауза
Time.timeScale = 1f; // Продолжить
```

**Важно**:
- UI работает даже на паузе
- Физика останавливается
- `Time.deltaTime` = 0

### 3. Wand.AddSlot перегрузка

```csharp
// Старый метод (принимает WandSlot)
public bool AddSlot(WandSlot slot);

// Новый метод (для RewardSystem)
public bool AddSlot(ScriptableObject data, SlotType type);
```

**Конвертация**: `ScriptableObject` → `SpellSO`/`BuffSO` → `WandSlot`

---

## 🎨 Кастомизация

### Изменить частоту наград

```csharp
// RewardSystem Inspector
Kills Per Reward: 15 // Вместо 10
```

### Изменить количество вариантов

```csharp
// RewardSystem Inspector
Options Count: 5 // Вместо 3
```

### Добавить больше наград

```csharp
// RewardSystem Inspector
Available Spells: +
Available Buffs: +
```

### Изменить цвета карточек

```csharp
// RewardCard Inspector
Spell Color: RGB(76, 128, 255) // Синий
Buff Color: RGB(255, 128, 76)  // Оранжевый
```

---

## 🐛 Troubleshooting

### Проблема: Панель не появляется после 10 убийств

**Решение**:
1. Проверить, что `RewardSystem` в сцене
2. Проверить `Reward Choice UI` назначен
3. Проверить Console на ошибки
4. Использовать `Debug: Add 5 Kills` для теста

### Проблема: Награды не добавляются в посох

**Решение**:
1. Проверить `Player Wand` назначен в `RewardSystem`
2. Проверить, что `maxSlots` не достигнут
3. Проверить Console: `[Wand] Slot added`

### Проблема: Игра не снимает паузу

**Решение**:
1. Проверить `Pause Game On Show` ✓ в `RewardChoiceUI`
2. Вручную: `Time.timeScale = 1` в Console
3. Проверить, что карточка была выбрана

### Проблема: Kill Counter не обновляется

**Решение**:
1. Проверить, что `KillCounterUI` в сцене
2. Проверить, что враги вызывают `OnAnyEnemyKilled`
3. Проверить, что убийца = Player (`killer.CompareTag("Player")`)

---

## ✅ Чеклист завершения Stage 10

- [x] **RewardOption.cs** - структура награды
- [x] **RewardSystem.cs** - логика наград
- [x] **RewardCard.cs** - компонент карточки
- [x] **RewardChoiceUI.cs** - панель выбора
- [x] **KillCounterUI.cs** - счётчик убийств
- [x] **Wand.AddSlot()** - перегрузка для SO
- [x] **EnemyController** - static event OnAnyEnemyKilled
- [x] **RewardCard.prefab** - префаб карточки
- [x] **RewardChoicePanel** - UI в HUDCanvas
- [x] **KillCounterPanel** - UI счётчика
- [x] **RewardSystem GameObject** - в сцене
- [x] **Тест 1**: Debug награда
- [x] **Тест 2**: Выбор награды
- [x] **Тест 3**: Счётчик убийств
- [x] **Тест 4**: Несколько наград

---

## 🎯 Что дальше?

Stage 10 завершён! Теперь можно переходить к **Stage 11: Полировка и интеграция**:
- Auto-fire при удержании ЛКМ
- Feedback при нехватке маны
- Cooldown индикатор
- Балансировка параметров
- Финальное тестирование

---

**Дата**: Октябрь 2025  
**Версия**: 1.0  
**Статус**: ✅ ЗАВЕРШЕНО
