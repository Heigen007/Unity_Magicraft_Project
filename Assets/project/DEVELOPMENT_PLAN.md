# План поэтапной разработки Magicraft

## Общая информация
- **Unity версия**: 2022.3 LTS или новее
- **Render Pipeline**: URP 2D
- **Input System**: New Input System
- **Стиль кода**: SOLID, ООП, 4 пробела для отступов

---

## Этап 0: Подготовка структуры проекта ✓
**Статус**: ЗАВЕРШЕН

### Задачи:
- [x] Создание структуры папок
- [ ] Базовые интерфейсы (IDamageable, ICaster, ISpellBehaviour)
- [ ] Утилиты (ObjectPool, Extensions)
- [ ] Базовые enum'ы и структуры данных

### Файлы:
- `Scripts/Player/IDamageable.cs`
- `Scripts/Combat/Wand/ICaster.cs`
- `Scripts/Combat/Wand/ISpellBehaviour.cs`
- `Scripts/Util/ObjectPool.cs`
- `Scripts/Util/Extensions.cs`

---

## Этап 1: Базовое движение и управление игроком
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] Настроить Input System (Actions: Move, Fire)
- [x] PlayerController с движением WASD
- [x] Поворот игрока к позиции мыши
- [x] Создать префаб Player с SpriteRenderer
- [x] CameraFollow для плавного следования камеры
- [x] PlayerInputHandler для обработки ввода

### Файлы:
- `Scripts/Player/PlayerController.cs` ✅
- `Scripts/Player/PlayerInputHandler.cs` ✅ (создаёт Input через код)
- `Scripts/Core/CameraFollow.cs` ✅
- `Prefabs/Player.prefab` (создается в Unity)

### Тестирование:
- Игрок двигается по WASD ✅
- Игрок поворачивается к курсору мыши ✅
- Движение плавное через Rigidbody2D ✅
- Камера следует за игроком ✅

**См. STAGE_1_COMPLETE.md для подробных инструкций по настройке в Unity**

---

## Этап 2: Система маны
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] ManaComponent (Current, Max, RegenPerSecond)
- [x] Методы TrySpend, Add, SetMana, FillToMax
- [x] События OnManaChanged, OnManaEmpty, OnManaFull, OnManaSpent
- [x] ManaBar UI с плавной анимацией
- [x] HUDCanvas для управления UI
- [x] Цветовая индикация (синий→красный)
- [x] Задержка перед регенерацией
- [x] Debug Context Menu команды

### Файлы:
- `Scripts/Player/ManaComponent.cs` ✅
- `Scripts/UI/ManaBar.cs` ✅
- `Scripts/UI/HUDCanvas.cs` ✅
- `Prefabs/UI_ManaBar.prefab` (создается в Unity)

### Тестирование:
- Мана отображается в UI ✅
- Мана регенерируется со временем ✅
- TrySpend корректно работает ✅
- Цвет меняется при низкой мане ✅
- Debug команды работают ✅

**См. STAGE_2_COMPLETE.md для подробных инструкций по настройке в Unity**

---

## Этап 3: Базовая система снарядов и стрельбы
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] Projectile класс (движение, время жизни, пробитие, OnTriggerEnter2D)
- [x] ProjectilePool с Object Pooling
- [x] SimpleShooter (временный) - стрельба по ЛКМ
- [x] CooldownTimer уже создан на Этапе 0
- [x] Привязка к ManaComponent
- [x] Реализация ICaster интерфейса
- [x] Debug информация (счётчики, статистика)

### Файлы:
- `Scripts/Combat/Projectiles/Projectile.cs` ✅
- `Scripts/Combat/Projectiles/ProjectilePool.cs` ✅
- `Scripts/Player/SimpleShooter.cs` ✅
- `Prefabs/Projectile_Basic.prefab` (создается в Unity)

### Тестирование:
- Клик ЛКМ создает снаряд ✅
- Снаряд летит к курсору ✅
- Кулдаун работает ✅
- Мана тратится ✅
- Pooling работает (снаряды переиспользуются) ✅
- Debug счётчики отображаются ✅

**См. STAGE_3_COMPLETE.md для подробных инструкций**

---

## Этап 4: Враги и спавнер
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] EnemyController (простое движение к игроку)
- [x] EnemySpawner (базовый спавн по таймеру)
- [x] EnemyPool
- [x] Базовая навигация (направление к игроку)

### Файлы:
- `Scripts/Enemies/EnemyController.cs` ✅
- `Scripts/Enemies/EnemySpawner.cs` ✅
- `Scripts/Enemies/EnemyPool.cs` ✅
- `Prefabs/Enemy_Basic.prefab` (создается в Unity)

### Тестирование:
- Враги спавнятся по таймеру ✅
- Враги двигаются к игроку ✅
- Pooling работает для врагов ✅

---

## Этап 5: Система урона и здоровья
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] HealthComponent (MaxHP, CurrentHP, ApplyDamage)
- [x] DamageDealer компонент для снарядов
- [x] Коллизии снаряд-враг
- [x] Смерть врагов (возврат в пул)
- [x] KillCounter
- [x] Здоровье игрока

### Файлы:
- `Scripts/Player/HealthComponent.cs` ✅
- `Scripts/Player/DamageDealer.cs` ✅
- `Scripts/Core/KillCounter.cs` ✅

### Тестирование:
- Снаряд попадает во врага - враг получает урон ✅
- Враг умирает при HP = 0 ✅
- KillCounter считает убийства ✅
- Игрок может получать урон от врагов ✅

---

## Этап 6: ScriptableObjects для контента
**Статус**: ЗАВЕРШЕН✅

### Задачи:
- [x] SpellSO (Id, DisplayName, BaseDamage, ManaCost, Speed, etc.) ✅
- [x] BuffSO (модификаторы) ✅
- [x] WandSO (базовые параметры посоха) ✅
- [x] Создать 2-3 тестовых Spell ✅
- [x] Создать 2-3 тестовых Buff ✅

### Файлы:
- `Scripts/Combat/Wand/SpellSO.cs` ✅
- `Scripts/Combat/Wand/BuffSO.cs` ✅
- `Scripts/Combat/Wand/WandSO.cs` ✅
### Тестирование:
- Можно создать SO через меню ✅
- SO корректно сохраняют данные ✅

---

## Этап 7: Система посоха и слотов (БЕЗ бафов)
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] Wand класс (слоты, TryCast без модификаторов) ✅
- [x] WandSlot структура ✅
- [x] SpellExecutor (исполнение заклинания) ✅
- [x] Простой UI списка слотов ✅

### Файлы:
- `Scripts/Combat/Wand/Wand.cs` ✅
- `Scripts/Combat/Wand/WandSlot.cs` ✅
- `Scripts/Combat/Wand/SpellExecutor.cs` ✅
- `Scripts/UI/WandSlotsUI.cs` ✅

### Тестирование:
- Wand стреляет заклинанием из слота ✅
- Заклинание создает снаряд с параметрами из SpellSO ✅
- UI показывает текущие слоты ✅

**См. STAGE_7_COMPLETE.md для подробных инструкций по настройке в Unity**
- Заклинание создает снаряд с параметрами из SpellSO
- UI показывает текущие слоты

---

## Этап 8: Система модификаторов и бафов
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] CastContext класс (финальные параметры каста) ✅
- [x] Алгоритм rightCumulative (бафы справа -> спеллы слева) ✅
- [x] BuildCastContextWithModifiers в Wand ✅
- [x] Поддержка фильтров по тегам ✅
- [x] Применение пассивных баффов из WandSO ✅
- [x] Debug методы для тестирования модификаторов ✅

### Файлы:
- `Scripts/Combat/Wand/CastContext.cs` (уже был готов)
- Обновление `Scripts/Combat/Wand/Wand.cs` ✅

### Тестирование:
- Баф справа усиливает спелл слева ✅
- Несколько бафов корректно стакаются ✅
- Изменение порядка слотов меняет результат ✅
- Фильтры по тегам работают ✅
- Пассивные баффы посоха применяются ✅

**См. STAGE_8_COMPLETE.md для подробных инструкций по настройке и тестированию в Unity**

---

## Этап 9: Drag & Drop редактор посоха
**Статус**: ЗАВЕРШЕН ✅

### Задачи:
- [x] WandEditorUI с Drag & Drop ✅
- [x] WandSlotUI - отдельный компонент слота ✅
- [x] Визуальные слоты (Spell/Buff иконки) ✅
- [x] Перестановка слотов через drag & drop ✅
- [x] Подсветка слотов при перетаскивании ✅
- [x] Автоматическое обновление при изменениях ✅

### Файлы:
- `Scripts/UI/WandEditorUI.cs` ✅
- `Scripts/UI/WandSlotUI.cs` ✅
- `Prefabs/UI_WandSlot.prefab` (создается в Unity)

### Тестирование:
- Можно перетащить слот ✅
- Порядок меняется ✅
- Модификаторы пересчитываются ✅
- Визуально отличаются Spell и Buff ✅

**См. STAGE_9_COMPLETE.md для подробных инструкций**

---

## Этап 10: Система наград
**Статус**: ОЖИДАЕТ

### Задачи:
- [ ] RewardSystem (слушает каждые 10 убийств)
- [ ] RewardChoiceUI (3 карточки выбора)
- [ ] Генерация случайных опций
- [ ] Пауза игры при выборе
- [ ] Добавление выбранного в посох

### Файлы:
- `Scripts/Rewards/RewardSystem.cs`
- `Scripts/Rewards/RewardOption.cs`
- `Scripts/UI/RewardChoiceUI.cs`
- `Prefabs/UI_RewardChoice.prefab`

### Тестирование:
- На 10, 20, 30... убийствах появляется меню
- Игра паузится
- Выбор применяется к посоху
- После выбора игра продолжается

---

## Этап 11: Полировка и интеграция
**Статус**: ОЖИДАЕТ

### Задачи:
- [ ] Auto-fire при удержании ЛКМ
- [ ] Feedback при нехватке маны
- [ ] Cooldown индикатор в UI
- [ ] Kill counter UI
- [ ] Балансировка параметров
- [ ] Финальное тестирование всех систем

### Файлы:
- Обновления в существующих файлах
- `Scripts/UI/CooldownIndicator.cs`
- `Scripts/UI/KillCounterUI.cs`

### Тестирование:
- Полный игровой цикл работает
- Все UI элементы на месте
- Нет критических багов

---

## Этап 12: Документация
**Статус**: ОЖИДАЕТ

### Задачи:
- [ ] README.md с инструкциями по Unity
- [ ] Гайд по созданию новых заклинаний
- [ ] Гайд по созданию новых бафов
- [ ] Список всех параметров SO

### Файлы:
- `README.md`
- `CONTENT_CREATION_GUIDE.md`

---

## Прогресс по этапам

- [x] Этап 0: Структура проекта ✅
- [x] Этап 1: Движение игрока ✅
- [x] Этап 2: Система маны ✅
- [x] Этап 3: Снаряды и стрельба ✅
- [x] Этап 4: Враги и спавнер ✅
- [x] Этап 5: Урон и здоровье ✅
- [x] Этап 6: ScriptableObjects ✅
- [x] Этап 7: Система посоха ✅
- [x] Этап 8: Бафы и модификаторы ✅
- [x] Этап 9: Редактор посоха ✅
- [ ] Этап 10: Награды
- [ ] Этап 11: Полировка
- [ ] Этап 12: Документация

**Общий прогресс**: 9/13 этапов завершено полностью (≈69%)

---

## Примечания

- Каждый этап должен быть полностью завершен и протестирован перед переходом к следующему
- При обнаружении багов - исправляем сразу
- Коммитим изменения после каждого этапа
- Поддерживаем 4 пробела для отступов
- Следуем SOLID принципам
