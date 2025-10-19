# ✅ ЭТАП 7: ЗАВЕРШЕН - Система посоха и слотов (БЕЗ бафов)

**Дата**: 19 января 2025  
**Статус**: ✅ Весь код реализован, требуется настройка в Unity Editor

---

## 📋 Что было реализовано в коде

Созданы 5 новых скриптов:

### ✅ 1. WandSlot.cs
**Файл**: `Scripts/Combat/Wand/WandSlot.cs`  
**Что делает**: Структура данных для хранения заклинания или бафа в слоте посоха

### ✅ 2. SpellExecutor.cs
**Файл**: `Scripts/Combat/Wand/SpellExecutor.cs`  
**Что делает**: Исполняет заклинания, создавая снаряды через ProjectilePool

### ✅ 3. Wand.cs
**Файл**: `Scripts/Combat/Wand/Wand.cs`  
**Что делает**: Основной компонент посоха - управляет слотами, кастами, кулдаунами

### ✅ 4. WandSlotsUI.cs
**Файл**: `Scripts/UI/WandSlotsUI.cs`  
**Что делает**: Визуальное отображение слотов посоха на экране

### ✅ 5. WandInputController.cs
**Файл**: `Scripts/Player/WandInputController.cs`  
**Что делает**: Связывает Wand с Input системой - при удержании ЛКМ вызывает каст

⚠️ **Важно**: SimpleShooter реализует ICaster (имеет свойства Muzzle, AimDirection, методы TrySpendMana). В этом этапе Wand использует SimpleShooter как ICaster.

---

## 🎮 Инструкции по настройке в Unity Editor

### Шаг 1: УДАЛИТЬ SimpleShooter с Player (ОБЯЗАТЕЛЬНО!)

⚠️ **ВАЖНО**: SimpleShooter должен быть УДАЛЕН, а не отключен!

1. Выберите **Player** в Hierarchy (НЕ в Play Mode!)
2. Найдите компонент **Simple Shooter** в Inspector
3. **ПКМ на Simple Shooter** → **Remove Component**
4. Подтвердите удаление

✅ **Результат**: SimpleShooter больше не будет стрелять вместо Wand

---

### Шаг 2: Добавить Wand компонент на Player

1. Player уже выбран в Hierarchy
2. Кликните **Add Component**
3. Найдите и добавьте **Wand** (Scripts/Combat/Wand)
4. В инспекторе Wand настройте:
   - **Wand Data**: перетащите `StarterWand.asset` из `Assets/project/ScriptableObjects/Wands/`
   - **Max Slots**: установите `6`
   - **Caster Component**: оставьте **None** (пока не нужен!)

⚠️ **ВАЖНО**: На Этапе 7 Wand работает БЕЗ ICaster! Caster Component оставьте пустым!

✅ **Результат**: На Player появится Wand компонент, который будет управлять заклинаниями

---

### Шаг 2: Добавить WandInputController на Player

1. Player уже выбран в Hierarchy
2. Кликните **Add Component**
3. Найдите и добавьте **Wand Input Controller** (Scripts/Player)
4. Никакие параметры настраивать не нужно - компонент автоматически найдет Wand и PlayerInputHandler

✅ **Результат**: Теперь удержание ЛКМ будет вызывать каст заклинаний через Wand

---

### Шаг 3: Настроить StarterWand.asset

1. В Project окне откройте `Assets/project/ScriptableObjects/Wands/StarterWand.asset`
2. В Inspector добавьте **2-3 заклинания** в массив **Starting Slots**:
   - Перетащите `MagicMissile.asset`
   - Перетащите `Fireball.asset`
   - Перетащите `Ice Spike.asset`
3. Установите **Global Cooldown**: `0.3`

✅ **Результат**: При загрузке игры в посохе будут 3 заклинания

---

### Шаг 4: Проверить наличие ProjectilePool в сцене

1. Hierarchy → найдите объект **ProjectilePool**
2. Если его нет - создайте:
   - **Hierarchy → Create Empty** → переименуйте в **ProjectilePool**
   - **Add Component → Projectile Pool**
   - **Projectile Prefab**: перетащите `Projectile_Basic.prefab` из `Assets/project/Prefabs/`
   - **Initial Pool Size**: `20`

✅ **Результат**: Снаряды будут создаваться через пул объектов

---

### Шаг 5 (ОПЦИОНАЛЬНО): Создать UI для отображения слотов

Если хотите видеть слоты посоха на экране:

#### 5.1. Создать префаб UI слота

1. **Hierarchy → UI → Image** → переименуйте в **WandSlot**
2. В RectTransform установите:
   - **Width**: `60`
   - **Height**: `60`
3. В Image компоненте:
   - **Color**: белый с Alpha `128`
4. **ПКМ на WandSlot → UI → Image** → переименуйте в **Icon**
   - **Width**: `50`, **Height**: `50`
   - Выровняйте по центру родителя (Anchors: Center)
5. **ПКМ на WandSlot → UI → Text - TextMeshPro** → переименуйте в **Text**
   - **Font Size**: `10`
   - **Alignment**: Center
   - Разместите под Icon
6. **Перетащите WandSlot** из Hierarchy в папку **Assets/project/Prefabs/** → сохранится как префаб
7. **Удалите WandSlot** из Hierarchy (префаб уже сохранен)

#### 5.2. Создать контейнер для слотов на Canvas

1. Найдите **HUD Canvas** в Hierarchy (или создайте Canvas если нет)
2. **ПКМ на HUD Canvas → UI → Panel** → переименуйте в **Wand Slots Panel**
3. В RectTransform настройте:
   - **Anchors**: нажмите квадрат слева снизу → выберите нижний центр (Bottom Center)
   - **Pivot**: `0.5, 0`
   - **Position**: X=`0`, Y=`80`
   - **Width**: `400`, **Height**: `80`
4. **Add Component → Layout → Horizontal Layout Group**:
   - **Spacing**: `10`
   - **Child Alignment**: Middle Center
   - **Child Force Expand**: включить Width ✓ и Height ✓
5. **Add Component → Wand Slots UI** (Scripts/UI)
6. В инспекторе WandSlotsUI настройте:
   - **Wand**: перетащите **Player** из Hierarchy
   - **Slot Prefab**: перетащите префаб **WandSlot** из Prefabs/
   - **Slots Container**: перетащите **Wand Slots Panel** (сам себя)
   - **Spell Color**: RGB(`77`, `153`, `255`) - синий
   - **Buff Color**: RGB(`255`, `204`, `51`) - золотой
   - **Active Color**: RGB(`255`, `255`, `255`) - белый
   - **Inactive Color**: RGB(`180`, `180`, `180`) - серый

✅ **Результат**: На экране в нижней части появятся визуальные слоты посоха

---

### Шаг 6: Финальная проверка компонентов на Player

Выберите Player и убедитесь, что на нем есть:

```
✅ PlayerController
✅ PlayerInputHandler
✅ ManaComponent
✅ HealthComponent (опционально)
✅ Rigidbody2D
✅ Collider2D
✅ Wand ⭐ (НОВЫЙ!)
✅ WandInputController ⭐ (НОВЫЙ!)
❌ SimpleShooter (УДАЛЕН!)
```

**Параметры Wand**:
- **Wand Data**: `StarterWand.asset` ✅
- **Max Slots**: `6` ✅
- **Caster Component**: `None (MonoBehaviour)` ✅ (оставьте пустым на Этапе 7!)

**Параметры Wand Input Controller**:
- Нет настроек (работает автоматически) ✅

---

### Шаг 7: Сделать посох видимым и привязать Muzzle к PlayerController

Если вы хотите, чтобы посох был видимым и касты шли именно от посоха (а не от центра игрока), выполните эти шаги (НЕ В PLAY MODE):

1. В Hierarchy выберите **Player**
2. **ПКМ → Create Empty** → переименуйте в **WandVisual** (будет дочерним элементом Player)
3. На **WandVisual** добавьте компонент **SpriteRenderer**
   - В поле **Sprite** выберите спрайт палки (если нет — временно используйте любой маленький прямоугольник)
   - Установите **Order in Layer** выше чем у игрока, если нужно
4. Добавьте на **WandVisual** компонент **WandVisual (Scripts/Player/WandVisual.cs)**
   - В поле **Caster Component** можно перетащить `Player` (или `PlayerController`) или оставить пустым — скрипт попытется найти `ICaster` автоматически
   - Настройте **Tip Distance** (расстояние от рукояти до кончика посоха). Подберите значение, пока кончик не будет на месте.
   - Если спрайт выглядит смещённым, откройте импорт спрайта и проверьте Pivot — рекомендуется ставить Pivot = Left или Center-Left для лучшего выравнивания.
5. Настройте позицию и поворот `WandVisual` так, чтобы палка выглядела в руках игрока (localPosition X ~ 0.4)
4. Поверните/сместите `WandVisual` так, чтобы она стояла в руках игрока (например, localPosition X=0.5, Y=0)
5. Создайте пустой объект-дочерний к `WandVisual`: **Внутри WandVisual → ПКМ → Create Empty** → переименуйте в **Muzzle**
   - Поместите `Muzzle` на конец палки (в том месте, откуда должен появляться снаряд)

6. Выберите **Player** → компонент **PlayerController**
   - В поле **Muzzle** перетащите `Player/WandVisual/Muzzle` (это установит `PlayerController.Muzzle`)

7. Выберите **Player** → компонент **Wand**
   - В поле **Caster Component** перетащите сам **Player** (GameObject) или конкретно компонент `PlayerController`

8. Сохраните сцену и запустите игру. При наведении мыши посох будет поворачиваться вслед за курсором (PlayerController обновляет AimDirection), а снаряды будут рождаться в позиции `Muzzle`.

✅ **Результат**: Видимая палка (WandVisual) поворачивается к мыши и выстрелы исходят из `Muzzle` на конце палки.

---

### Финальная проверка: Присвоение PlayerController как Caster

1. (Опционально) Убедитесь, что в **Wand → Caster Component** установлен `PlayerController` (если вы хотите, чтобы посох использовал реальные мана/минимумы и Muzzle из Player)
2. Если установлен `PlayerController`, при старте в Console вы увидите: `[Wand] Caster found: Magicraft.Player.PlayerController` или подобное
3. Play Mode → зажмите ЛКМ → в Console должно быть: `[Wand] Casting: <DisplayName>` и снаряд должен появляться из `Player/WandVisual/Muzzle`



## 🧪 Тестирование в Play Mode

### ⚠️ ВАЖНО: Диагностика перед тестированием

**Проблема 1**: Вылетает 3 снаряда за раз вместо 1?

**Причина**: У вас работают сразу несколько систем стрельбы одновременно:
1. SimpleShooter (старая система)
2. Wand (новая система)
3. Возможно еще какой-то компонент

**Решение**:
1. **Play Mode** → выберите Player в Hierarchy
2. В Inspector найдите **Simple Shooter** компонент
3. **ОТКЛЮЧИТЕ галочку** слева от названия компонента (НЕ удаляйте!)
4. Оставьте включенным только **Wand** и **Wand Input Controller**

✅ **Теперь стрелять должен только Wand!**

---

**Проблема 2**: В Console нет логов `[Wand] Casting...` → значит стреляет НЕ Wand!

**Пошаговая диагностика**:

1. **Остановите игру** (Exit Play Mode)

2. **Проверьте Player в Inspector** (в Edit Mode, НЕ Play Mode):
   - ✅ Есть компонент **Wand**?
   - ✅ Есть компонент **Wand Input Controller**?
   - ✅ Компонент **Simple Shooter** ОТКЛЮЧЕН (галочка убрана)?

3. **Проверьте настройки Wand**:
   - Выберите Player → Wand компонент
   - **Wand Data**: установлен `StarterWand.asset`?
   - **Caster Component**: установлен `Simple Shooter` (компонент)?
   - **Max Slots**: стоит `6` или больше?

4. **Проверьте StarterWand.asset**:
   - Project → `ScriptableObjects/Wands/StarterWand`
   - **Starting Slots**: есть хотя бы 1 заклинание?
   - Если пусто → добавьте `MagicMissile.asset`

5. **Запустите игру снова** и проверьте Console при стрельбе

**Если всё ещё не работает** - скажи мне:
- Есть ли компонент Wand на Player?
- Установлен ли Wand Data?
- Есть ли Wand Input Controller?

---

### Тест 1: Базовая стрельба
1. **Play** → запустите игру
2. **Зажмите ЛКМ**
3. ✅ Ожидание: **ОДИН снаряд** за раз (не 3!)
4. ✅ Мана тратится (смотрите на ManaBar)
5. ✅ Есть кулдаун между выстрелами
6. ✅ В Console нет ошибок

### Тест 2: Переключение слотов
1. **Play** → запустите игру
2. Сделайте 3 выстрела
3. ✅ Ожидание: Каждый выстрел использует следующее заклинание из StarterWand
4. ✅ Если создали UI - подсветка активного слота перемещается

### Тест 3: Разные заклинания
1. В `StarterWand.asset` убедитесь что есть разные заклинания
2. **Play** → стреляйте
3. ✅ Ожидание: Снаряды чередуются (разная скорость/урон/цвет)

### Тест 4: Пустой посох
1. В `StarterWand.asset` удалите все заклинания (Size = 0)
2. **Play** → зажмите ЛКМ
3. ✅ Ожидание: Стрельба не происходит, в Console: "No spell to cast!"

### Тест 5: UI слотов (если создали)
1. **Play** → запустите игру
2. ✅ UI показывает все слоты
3. ✅ Заклинания синего цвета
4. ✅ Активный слот белого цвета
5. ✅ При касте - подсветка переключается

---

## 🔍 Как понять что всё работает правильно?

### ✅ ШАГ 1: Проверка что стреляет Wand (а не SimpleShooter)

1. **Запустите игру** (Play Mode)
2. **Откройте Console**: Menu → Window → General → Console
3. **Зажмите ЛКМ** → стреляйте
4. **Смотрите в Console**:
   - ✅ **Если Wand работает**: Увидите `[Wand] Casting: <название заклинания>`
   - ❌ **Если SimpleShooter**: Ничего не будет, или другие сообщения
   - ⚠️ **Если ошибка**: Увидите красные сообщения - сообщи мне!

**Почему посоха не видно?**  
Wand - это **невидимый компонент** (как ManaComponent). Он не имеет спрайта. Если хочешь видеть посох - нужен отдельный спрайт на Player (это будет потом).

---

### ✅ ШАГ 2: Признаки правильной работы Wand системы

1. **Один снаряд за каст**
   - При зажатии ЛКМ вылетает **1 снаряд**, не 2-3 ✅
   - Если вылетает несколько - отключите SimpleShooter (см. "Диагностика" выше)

2. **Переключение слотов**
   - Сделайте 3 выстрела
   - Каждый выстрел должен быть **разным заклинанием** (если в StarterWand 3+ заклинания)
   - После последнего слота - возврат к первому (циклично)

3. **Консоль при стрельбе** (ГЛАВНАЯ ПРОВЕРКА!)
   - Откройте **Window → General → Console**
   - При каждом выстреле должно быть: `[Wand] Casting: <название заклинания>`
   - Если пустой посох: `[Wand] No spell to cast!`

4. **Мана тратится**
   - Посмотрите на ManaBar (синяя полоса)
   - После каждого выстрела мана уменьшается
   - Когда мана кончается - стрельба прекращается

5. **Кулдаун работает**
   - Зажмите ЛКМ
   - Снаряды вылетают с паузами (0.3 сек)
   - Не моментально все сразу

6. **UI слотов (если создали)**
   - Видны все слоты внизу экрана
   - Активный слот **белого** цвета
   - Заклинания **синие**, бафы **золотые**
   - При выстреле подсветка перемещается вправо

---

### ❌ Признаки проблем:

| Проблема | Причина | Решение |
|----------|---------|---------|
| **3 снаряда за раз** | Работают SimpleShooter + Wand + еще что-то | Отключите SimpleShooter, проверьте другие компоненты на Player |
| **Ошибка "Caster is null"** | Не установлен Caster Component в Wand | Player → Wand → Caster Component = SimpleShooter |
| **Стрельба не работает** | Нет WandInputController | Player → Add Component → Wand Input Controller |
| **Всегда одно заклинание** | В StarterWand только 1 заклинание | StarterWand.asset → Starting Slots → добавьте 2-3 заклинания |
| **Мана не тратится** | SimpleShooter не связан с ManaComponent | Проверьте что ManaComponent есть на Player |
| **UI не показывается** | Не установлены ссылки | Wand Slots Panel → WandSlotsUI → проверьте все поля |

---

## �🐛 Возможные проблемы и решения

### ❌ Проблема: "Caster is null!" в Console

**Причина**: В Wand компоненте не установлен Caster Component

**Решение**:
1. Player → Wand компонент
2. **Caster Component**: перетащите **Simple Shooter** компонент с Player

---

### ❌ Проблема: Снаряды не создаются

**Причина**: ProjectilePool не существует в сцене

**Решение**:
1. Hierarchy → Create Empty → "ProjectilePool"
2. Add Component → **Projectile Pool**
3. **Projectile Prefab**: `Projectile_Basic.prefab`
4. **Initial Pool Size**: `20`

---

### ❌ Проблема: Стрельба не работает

**Причина**: WandInputController не добавлен

**Решение**:
1. Player → Add Component → **Wand Input Controller**

---

### ❌ Проблема: UI слотов не отображается

**Причина**: Не установлены ссылки в WandSlotsUI

**Решение**:
1. Wand Slots Panel → WandSlotsUI компонент
2. Проверьте:
   - **Wand**: должен быть Player
   - **Slot Prefab**: префаб WandSlot
   - **Slots Container**: Wand Slots Panel (Transform)

---

### ❌ Проблема: Мана не тратится

**Причина**: SimpleShooter не связан с ManaComponent

**Решение**:
1. Player → Simple Shooter
2. Убедитесь что ManaComponent есть на Player
3. SimpleShooter автоматически найдет ManaComponent в Awake()

---

## ⚠️ Важные замечания

### Что работает на Этапе 7:
- ✅ Система слотов с заклинаниями
- ✅ Автоматическое переключение слотов после каста
- ✅ Загрузка из WandSO
- ✅ UI отображение слотов
- ✅ Кулдауны

### Что НЕ работает (будет на Этапе 8):
- ❌ **Бафы не применяют модификаторы** (бафы в слотах игнорируются)
- ❌ Алгоритм `rightCumulative` (модификаторы справа)
- ❌ Drag & Drop редактор слотов
- ❌ Ручное переключение слотов по Q/E

---

## 🎯 Следующие шаги

**Этап 8** добавит:
1. Алгоритм `RecomputeCumulativeModifiers()` - бафы справа влияют на заклинания слева
2. Применение модификаторов к урону/скорости/дальности
3. Фильтры по тегам (`CanAffectSpell`)
4. Стакание модификаторов

После Этапа 8 бафы заработают! 🔥

---

## ✅ Чек-лист завершения Этапа 7

**Код** (все готово ✅):
- [x] WandSlot.cs
- [x] SpellExecutor.cs
- [x] Wand.cs
- [x] WandSlotsUI.cs
- [x] WandInputController.cs

**Unity Editor** (требуется настройка):
- [ ] Wand добавлен на Player
- [ ] Caster Component установлен (Simple Shooter)
- [ ] WandInputController добавлен на Player
- [ ] StarterWand.asset содержит 2-3 заклинания
- [ ] ProjectilePool существует в сцене
- [ ] UI слотов создан (опционально)
- [ ] Протестирована стрельба
- [ ] Протестировано переключение слотов

---

**Этап 7 завершен!** 🎉

Все скрипты созданы. Теперь настройте компоненты в Unity Editor согласно инструкциям выше.
