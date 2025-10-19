# 🎯 Этап 4: Враги и система урона - Инструкции

## ✅ Созданные скрипты:

1. **HealthComponent.cs** - компонент здоровья (HP, регенерация, события)
2. **EnemyController.cs** - AI врага (движение к игроку)
3. **EnemyPool.cs** - пулинг врагов
4. **EnemySpawner.cs** - спавнер волнами

---

## 🔧 Настройка в Unity:

### 1. Создать префаб врага

#### Шаг 1: Создать GameObject врага
1. Hierarchy → ПКМ → Create Empty → "Enemy_Basic"
2. Добавь компоненты:

**SpriteRenderer:**
- Sprite: любой (Create → Sprites → Circle или Square)
- Color: Красный (255, 0, 0)
- Sorting Layer: Default
- Order in Layer: 5

**Circle Collider 2D:**
- Radius: 0.3
- **Is Trigger: НЕТ** (выключи галочку)

**Rigidbody 2D:**
- Body Type: **Dynamic**
- Gravity Scale: 0
- Constraints → Freeze Rotation Z: ✅ (поставь галочку)
- **Simulated: ✅ ВКЛЮЧЕНО**

**HealthComponent:**
- Max Health: 30
- Start Health: 0 (auto = max)
- Health Regen: 0
- Regen Delay: 3
- Invulnerability Duration: 0

**EnemyController:**
- Move Speed: 3
- Stop Distance: 0.5
- Target: оставь пустым (найдётся автоматически)
- Sprite Renderer: перетащи SpriteRenderer

3. **Layer:** Default (или создай отдельный "Enemy")
4. **Tag:** можно создать "Enemy" (опционально)

#### Шаг 2: Сохранить как префаб
1. Перетащи Enemy_Basic из Hierarchy → в папку Prefabs
2. Удали Enemy_Basic из сцены

---

### 2. Настроить EnemySpawner

#### Создать GameObject спавнера:
1. Hierarchy → ПКМ → Create Empty → "EnemySpawner"
2. Add Component → **EnemySpawner**

**Настройки:**
- Spawn Interval: 2 (врагов каждые 2 секунды)
- Enemies Per Spawn: 1
- Max Enemies: 20
- Min Spawn Distance: 5
- Max Spawn Distance: 10
- Target: перетащи **Player** из Hierarchy
- Enemy Pool: оставь пустым (найдётся автоматически)

---

### 3. Создать EnemyPool

**Вариант A: Автоматическое создание** (рекомендуется)
- EnemySpawner автоматически найдёт/создаст EnemyPool при запуске

**Вариант B: Ручное создание** (если нужен контроль)
1. Hierarchy → ПКМ → Create Empty → "EnemyPool"
2. Add Component → **EnemyPool**
3. Enemy Prefab: перетащи **Enemy_Basic** из Prefabs
4. Initial Pool Size: 30

---

### 4. Настроить Player для получения урона (опционально)

Если хочешь, чтобы враги могли атаковать игрока:

1. Выбери **Player** в Hierarchy
2. Add Component → **HealthComponent**
3. Настройки:
   - Max Health: 100
   - Health Regen: 1 (восстановление 1 HP/сек)
   - Regen Delay: 5 (начинается через 5 сек после урона)
   - Invulnerability Duration: 0.5 (неуязвимость после урона)

---

### 5. Настроить слои коллизий (ВАЖНО!)

Чтобы снаряды не попадали в игрока, а враги могли:

1. Edit → Project Settings → Physics 2D
2. Layer Collision Matrix:
   - **Default** (Player) ✅ коллизия с **Default** (Enemy)
   - **Default** (Projectile) ✅ коллизия с **Default** (Enemy)
   - **Default** (Projectile) ❌ НЕТ коллизии с **Default** (Player)

**Или создай отдельные слои:**
1. Edit → Project Settings → Tags and Layers
2. Layers:
   - Layer 6: Player
   - Layer 7: Enemy
   - Layer 8: Projectile

3. Назначь слои:
   - Player GameObject → Layer: Player
   - Enemy_Basic префаб → Layer: Enemy
   - Projectile_Basic префаб → Layer: Projectile

4. Physics 2D → Layer Collision Matrix:
   - Projectile ✅ Enemy
   - Projectile ❌ Player
   - Enemy ✅ Player

---

## 🎮 Тестирование:

### 1. Запусти игру
- Должны заспавниться враги вокруг игрока (каждые 2 секунды)

### 2. Зажми ЛКМ
- Стреляй по врагам

### 3. Что должно происходить:
- ✅ Враги появляются вокруг игрока (на расстоянии 5-10 единиц)
- ✅ Враги двигаются к игроку (скорость 3)
- ✅ Снаряды попадают во врагов
- ✅ **Над врагами появляется HP** (красный текст "30/30")
- ✅ **HP уменьшается** при попадании снаряда
- ✅ Враг **исчезает** когда HP = 0
- ✅ Враг становится **серым** перед исчезновением

### 4. Debug информация:
В левом верхнем углу экрана:
```
Mana: X/100
Projectile Pool: X available / 20 total
Enemy Pool: X available / 30 total
Active Enemies: X / 20
```

---

## ⚙️ Параметры для баланса:

### Enemy_Basic:
- **Max Health: 30** = убивается за 3 выстрела (урон снаряда 10)
- **Move Speed: 3** = медленнее игрока (5)

### EnemySpawner:
- **Spawn Interval: 2** = каждые 2 секунды
- **Max Enemies: 20** = не более 20 одновременно

### Projectile:
- **Damage: 10** (SimpleShooter → baseDamage)
- **Speed: 12**
- **Mana Cost: 5**
- **Cooldown: 0.3**

---

## 🐛 Если что-то не работает:

### Враги не появляются:
1. Проверь Console - есть ли ошибки?
2. EnemySpawner → Target назначен?
3. Enemy Prefab создан и назначен?

### Снаряды не наносят урон:
1. У Enemy_Basic есть **HealthComponent**?
2. Collider **Is Trigger = НЕТ**?
3. Слои коллизий настроены?

### Враги не двигаются:
1. Rigidbody **Simulated = ДА**?
2. EnemyController → Target назначен?
3. Move Speed > 0?

### HP не отображается:
- Это нормально! OnGUI показывает красный текст над врагом
- Если не видно - возможно враг за пределами экрана

---

## 📊 Следующие улучшения (Этап 5+):

- Разные типы врагов (ScriptableObjects)
- Система волн с усложнением
- Награды за убийство
- Анимации врагов
- Звуковые эффекты
- UI здоровья врагов (полоски)

**Готов тестировать?** 🎯💀
