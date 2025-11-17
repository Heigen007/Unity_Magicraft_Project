# Stage 11: Добавление анимаций

## 📋 Обзор

Добавляем анимации для:
- **Посох** - иконка вместо палочки
- **Игрок** - анимация ходьбы
- **3 новых заклинания** - появляются в точке посоха (не летят)
  - Grenade (Explosion_1)
  - Atomic Bomb (Explosion_3)
  - Water Splash (Explosion_6)

## ⚡ Быстрый старт

### Код уже готов:
- ✅ `SpellEffect.cs` - создан
- ✅ `SpellExecutor.cs` - обновлён (поддержка AoE)
- ✅ `PlayerController.cs` - обновлён (анимация)

### Что делать в Unity:

1. **Импортировать спрайты** (настроить Texture Type: Sprite 2D, Filter: Point)
2. **Создать анимации** (Animation window)
3. **Создать Animator Controllers**
4. **Создать префабы для заклинаний**
5. **Создать ScriptableObjects** (Grenade, Atomic Bomb, Water Splash)
6. **Протестировать**

---

## 🎨 Часть 1: Анимация игрока

### 1. Импорт спрайтов игрока

1. **Unity** → **Project** → папка `Animations/wizardSprites/PNG/wizard/`

2. **Выбрать все файлы `2_WALK_000.png` до `2_WALK_004.png`** (5 файлов)

3. **Inspector** → настроить:
   - Texture Type: **Sprite (2D and UI)**
   - Sprite Mode: **Single**
   - Pixels Per Unit: `100`
   - Filter Mode: **Point (no filter)** (для пиксель-арт стиля)
   - Compression: **None**
   - Нажать **Apply**

### 2. Создать Animation Clip для ходьбы

1. **Hierarchy** → выбрать **Player**

2. **Window** → **Animation** → **Animation**

3. **В окне Animation** → нажать **Create**

4. Сохранить как `Animations/Player_Walk.anim`

5. **В Animation окне**:
   - Нажать **Add Property** → `Sprite Renderer` → `Sprite`
   - В Timeline перетащить 5 спрайтов `2_WALK_000` до `2_WALK_004` по порядку
   - Установить Frame Rate: **10 FPS** (справа вверху)
   - Включить **Loop** (галочка в инспекторе анимации)

### 3. Создать Idle анимацию

1. **В окне Animation** → выбрать **Create New Clip**

2. Сохранить как `Animations/Player_Idle.anim`

3. Перетащить спрайт `1_IDLE_000.png` в Timeline

4. Установить Frame Rate: **10 FPS**

### 4. Настроить Animator Controller

1. **Hierarchy** → **Player** → **Add Component** → **Animator**

2. **Project** → ПКМ в папке `Animations/` → **Create** → **Animator Controller**

3. Назвать `Player_AnimatorController`

4. **Player** → **Animator** → **Controller** → перетащить `Player_AnimatorController`

5. **Двойной клик** на `Player_AnimatorController`

6. **В Animator окне**:
   - ПКМ → **Create State** → **Empty**
   - Назвать `Idle`
   - Перетащить `Player_Idle` в `Idle` state
   - ПКМ на `Idle` → **Set as Layer Default State** (оранжевый)
   
   - ПКМ → **Create State** → **Empty**
   - Назвать `Walk`
   - Перетащить `Player_Walk` в `Walk` state

7. **Создать переходы**:
   - ПКМ на `Idle` → **Make Transition** → кликнуть на `Walk`
   - ПКМ на `Walk` → **Make Transition** → кликнуть на `Idle`

8. **Создать параметр**:
   - Слева вверху **Parameters** → `+` → **Bool**
   - Назвать `IsMoving`

9. **Настроить переходы**:
   - Кликнуть на стрелку `Idle → Walk`
   - **Inspector** → **Conditions** → `+` → `IsMoving` = `true`
   - **Settings** → **Has Exit Time** = `false`
   - **Transition Duration** = `0`
   
   - Кликнуть на стрелку `Walk → Idle`
   - **Inspector** → **Conditions** → `+` → `IsMoving` = `false`
   - **Settings** → **Has Exit Time** = `false`
   - **Transition Duration** = `0`

---

## 🪄 Часть 2: Анимация посоха (иконка)

### 1. Импорт спрайта посоха

1. **Unity** → **Project** → папка `Animations/wandSprites/PNG/Staves_1/`

2. **Выбрать файл `1.png`**

3. **Inspector** → настроить:
   - Texture Type: **Sprite (2D and UI)**
   - Sprite Mode: **Single**
   - Pixels Per Unit: `100`
   - Filter Mode: **Point (no filter)**
   - Compression: **None**
   - Нажать **Apply**

### 2. Заменить спрайт посоха

1. **Hierarchy** → **Player** → найти дочерний объект с посохом (если есть)

2. Если нет визуала посоха:
   - ПКМ на **Player** → **2D Object** → **Sprite**
   - Назвать `WandVisual`
   - **Transform**:
     - Position: `(0.3, 0, 0)` (справа от игрока)
     - Rotation: `(0, 0, -45)` (под углом)
     - Scale: `(0.5, 0.5, 1)`

3. **Sprite Renderer** → **Sprite** → перетащить `1.png`

4. **Sorting Layer**: установить выше игрока (Order in Layer = `10`)

---

## 💥 Часть 3: Новые заклинания с анимацией

### 1. Импорт спрайтов заклинаний

#### Grenade (Explosion_1)

1. **Unity** → **Project** → `Animations/spellsSprites/PNG/Explosion_1/`

2. **Выбрать все 10 файлов** (`Explosion_1.png` до `Explosion_10.png`)

3. **Inspector** → настроить:
   - Texture Type: **Sprite (2D and UI)**
   - Sprite Mode: **Single**
   - Pixels Per Unit: `100`
   - Filter Mode: **Point (no filter)**
   - Compression: **None**
   - Нажать **Apply**

#### Atomic Bomb (Explosion_3)

1. **Папка** `Animations/spellsSprites/PNG/Explosion_3/`

2. Импортировать все 10 файлов с теми же настройками

#### Water Splash (Explosion_6)

1. **Папка** `Animations/spellsSprites/PNG/Explosion_6/`

2. Импортировать все 10 файлов с теми же настройками

---

### 2. Создать префабы для новых заклинаний

#### Prefab: Spell_Grenade

1. **Hierarchy** → ПКМ → **2D Object** → **Sprite**

2. Назвать `Spell_Grenade`

3. **Add Component** → **Animator**

4. **Add Component** → **Circle Collider 2D**
   - Is Trigger: ✓
   - Radius: `0.5`

5. **Add Component** → **Spell Effect** (создадим скрипт позже)

6. **Создать Animation**:
   - **Window** → **Animation** → **Animation**
   - **Create** → сохранить как `Animations/Grenade_Explode.anim`
   - Перетащить все 10 спрайтов `Explosion_1` по порядку
   - Frame Rate: **15 FPS**
   - **Loop**: выключить (играет один раз)

7. **Создать Animator Controller**:
   - **Project** → ПКМ в `Animations/` → **Create** → **Animator Controller**
   - Назвать `Grenade_AnimatorController`
   - **Spell_Grenade** → **Animator** → **Controller** → перетащить контроллер
   - Открыть контроллер двойным кликом
   - Перетащить `Grenade_Explode` анимацию в окно
   - Она автоматически станет дефолтным состоянием

8. **Перетащить в Project** → `Prefabs/Spell_Grenade.prefab`

9. **Удалить из Hierarchy**

#### Prefab: Spell_AtomicBomb

Повторить те же шаги для `Explosion_3`:
- Префаб: `Spell_AtomicBomb`
- Animation: `Animations/AtomicBomb_Explode.anim`
- Controller: `AtomicBomb_AnimatorController`
- Collider Radius: `1.0` (больше)
- Frame Rate: **12 FPS** (медленнее)

#### Prefab: Spell_WaterSplash

Повторить для `Explosion_6`:
- Префаб: `Spell_WaterSplash`
- Animation: `Animations/WaterSplash_Explode.anim`
- Controller: `WaterSplash_AnimatorController`
- Collider Radius: `0.7`
- Frame Rate: **15 FPS**

---

### 3. Создать ScriptableObjects для новых заклинаний

#### Grenade.asset

1. **Project** → `ScriptableObjects/Spells/`

2. **ПКМ** → **Create** → **Magicraft** → **Spell**

3. Назвать `Grenade`

4. **Inspector**:
   - **Identity**:
     - Id: `grenade`
     - Display Name: `Grenade`
     - Icon: (любая иконка или оставить пустым)
     - Description: `Throws a grenade that explodes on impact`
   
   - **Execution**:
     - Execution Type: **AoE**
     - Projectile Prefab: `Spell_Grenade.prefab`
   
   - **Base Stats**:
     - Base Damage: `25`
     - Base Mana Cost: `15`
     - Base Cooldown: `1.5`
   
   - **Projectile Stats**:
     - Projectile Speed: `0` (не летит, появляется на месте)
     - Range: `1.5` (радиус взрыва)
     - Pierce: `0`
   
   - **Tags**:
     - `Fire`
     - `AoE`

#### Atomic Bomb.asset

1. **Create** → **Magicraft** → **Spell**

2. Назвать `Atomic Bomb`

3. **Inspector**:
   - Id: `atomic_bomb`
   - Display Name: `Atomic Bomb`
   - Description: `Devastating explosion with massive area damage`
   - Execution Type: **AoE**
   - Projectile Prefab: `Spell_AtomicBomb.prefab`
   - Base Damage: `50`
   - Base Mana Cost: `30`
   - Base Cooldown: `3.0`
   - Range: `2.5` (большой радиус)
   - Tags: `Fire`, `AoE`

#### Water Splash.asset

1. **Create** → **Magicraft** → **Spell**

2. Назвать `Water Splash`

3. **Inspector**:
   - Id: `water_splash`
   - Display Name: `Water Splash`
   - Description: `Splashes water dealing area damage`
   - Execution Type: **AoE**
   - Projectile Prefab: `Spell_WaterSplash.prefab`
   - Base Damage: `15`
   - Base Mana Cost: `10`
   - Base Cooldown: `0.8`
   - Range: `1.2`
   - Tags: `Ice`, `AoE`

---

### 5. Обновить SpellExecutor для AoE заклинаний

Нужно добавить поддержку AoE типа в `SpellExecutor.cs`:

```csharp
// В методе Execute()
public void Execute(CastContext context)
{
    if (context.SourceSpell.ExecutionType == SpellExecutionType.Projectile)
    {
        ExecuteProjectile(context);
    }
    else if (context.SourceSpell.ExecutionType == SpellExecutionType.AoE)
    {
        ExecuteAoE(context);
    }
    else
    {
        Debug.LogWarning($"[SpellExecutor] Execution type {context.SourceSpell.ExecutionType} not implemented!");
    }
}

// Новый метод для AoE
private void ExecuteAoE(CastContext context)
{
    if (context.SourceSpell.ProjectilePrefab == null)
    {
        Debug.LogError("[SpellExecutor] AoE spell has no prefab!");
        return;
    }

    // Создать эффект в точке мышки (или можно в точке кастера)
    Vector2 spawnPos = context.SpawnPosition;
    
    // Если нужно спавнить в точке мыши:
    // Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    // spawnPos = mouseWorldPos;

    GameObject effectObj = GameObject.Instantiate(
        context.SourceSpell.ProjectilePrefab,
        spawnPos,
        Quaternion.identity
    );

    SpellEffect effect = effectObj.GetComponent<SpellEffect>();
    if (effect != null)
    {
        effect.SetLifetime(context.Range / 2f); // Используем Range как время жизни
        effect.SetDamageRadius(context.Range);
        effect.Initialize(context, (e) => GameObject.Destroy(e.gameObject));
    }
    else
    {
        Debug.LogError("[SpellExecutor] AoE prefab has no SpellEffect component!");
    }
}
```

---

## 🧪 Тестирование

### Тест 1: Анимация игрока

1. **Play Mode**

2. **Двигаться WASD**

3. **Проверить**:
   - Игрок показывает анимацию ходьбы
   - При остановке - Idle (статичный)

✅ **Результат**: Анимация плавно переключается

---

### Тест 2: Иконка посоха

1. **Play Mode**

2. **Проверить**:
   - Посох отображается как иконка (не палочка)
   - Поворачивается с игроком

✅ **Результат**: Посох виден и правильно расположен

---

### Тест 3: Новые заклинания

1. **Wand** → **Inspector** → **Slots**

2. **Добавить слот**: перетащить `Grenade.asset`

3. **Play Mode** → **ЛКМ**

4. **Проверить**:
   - Эффект появляется в точке посоха
   - Проигрывается анимация взрыва
   - Наносится урон врагам в радиусе
   - Эффект исчезает после анимации

✅ **Результат**: Заклинание работает, анимация проигрывается

---

### Тест 4: Все три новых заклинания

Повторить для `Atomic Bomb` и `Water Splash`

✅ **Результат**: Все три заклинания работают с анимациями

---

## ✅ Чеклист завершения

- [ ] Импортированы спрайты игрока (walk)
- [ ] Создана анимация Player_Walk
- [ ] Создана анимация Player_Idle
- [ ] Настроен Animator Controller игрока
- [ ] Обновлён PlayerController.cs
- [ ] Импортирован спрайт посоха
- [ ] Создан WandVisual объект
- [ ] Импортированы спрайты Explosion_1
- [ ] Импортированы спрайты Explosion_3
- [ ] Импортированы спрайты Explosion_6
- [ ] Создан префаб Spell_Grenade
- [ ] Создан префаб Spell_AtomicBomb
- [ ] Создан префаб Spell_WaterSplash
- [ ] Создан скрипт SpellEffect.cs
- [ ] Создан SO Grenade.asset
- [ ] Создан SO Atomic Bomb.asset
- [ ] Создан SO Water Splash.asset
- [ ] Обновлён SpellExecutor.cs
- [ ] Протестирована анимация игрока
- [ ] Протестирована иконка посоха
- [ ] Протестированы новые заклинания

---

## 📝 Примечания

- **Frame Rate** анимаций можно настроить по вкусу
- **Радиусы** эффектов можно менять в префабах
- **Время жизни** эффектов настраивается в SpellEffect компоненте
- Старые заклинания (Fireball, Ice Spike, Magic Missile) остаются как есть

---

**Дата**: Ноябрь 2025  
**Версия**: 1.0  
**Статус**: 🔧 В РАБОТЕ
