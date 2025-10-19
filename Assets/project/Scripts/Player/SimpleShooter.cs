using UnityEngine;
using Magicraft.Player;

namespace Magicraft.Combat
{
    /// <summary>
    /// Простой компонент для стрельбы (временный, до полной системы Wand)
    /// Позволяет стрелять снарядами по клику ЛКМ
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(ManaComponent))]
    public class SimpleShooter : MonoBehaviour, ICaster
    {
        [Header("Shooting Settings")]
        [Tooltip("Базовый урон снаряда")]
        [SerializeField] private float baseDamage = 10f;

        [Tooltip("Стоимость выстрела в мане")]
        [SerializeField] private float manaCost = 5f;

        [Tooltip("Кулдаун между выстрелами")]
        [SerializeField] private float cooldown = 0.3f;

        [Tooltip("Скорость снаряда")]
        [SerializeField] private float projectileSpeed = 12f;

        [Tooltip("Дальность полёта")]
        [SerializeField] private float range = 10f;

        [Header("References")]
        [Tooltip("Точка выхода снаряда (создайте пустой объект впереди игрока)")]
        [SerializeField] private Transform muzzle;

        [Tooltip("Префаб снаряда (для создания пула)")]
        [SerializeField] private Projectiles.Projectile projectilePrefab;

        // Компоненты
        private PlayerController playerController;
        private ManaComponent manaComponent;
        private PlayerInputHandler inputHandler;
        private CooldownTimer cooldownTimer;

        // Пул снарядов
        private Projectiles.ProjectilePool projectilePool;

        // Статистика
        private int shotsFired;

        // ICaster implementation
        public Transform Muzzle => muzzle != null ? muzzle : transform;
        public Vector2 AimDirection => playerController != null ? playerController.AimDirection : Vector2.right;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            manaComponent = GetComponent<ManaComponent>();
            inputHandler = GetComponent<PlayerInputHandler>();

            cooldownTimer = new CooldownTimer();

            // Создать muzzle если не задан
            if (muzzle == null)
            {
                GameObject muzzleObj = new GameObject("Muzzle");
                muzzle = muzzleObj.transform;
                muzzle.SetParent(transform);
                muzzle.localPosition = Vector3.right * 0.5f; // Немного впереди игрока
            }

            // Создать пул снарядов
            if (projectilePrefab != null)
            {
                CreateProjectilePool();
            }
        }

        private void OnEnable()
        {
            // Стрельба работает через проверку IsFireHeld в Update
        }

        private void OnDisable()
        {
            // Ничего не делаем
        }

        private void Update()
        {
            cooldownTimer.Update(Time.deltaTime);

            // Auto-fire при удержании ЛКМ
            if (inputHandler != null && inputHandler.IsFireHeld)
            {
                TryShoot();
            }
        }

        /// <summary>
        /// Создать пул снарядов
        /// </summary>
        private void CreateProjectilePool()
        {
            // Проверяем, есть ли уже ProjectilePool в сцене
            projectilePool = Projectiles.ProjectilePool.Instance;

            if (projectilePool == null)
            {
                // Создать новый GameObject с ProjectilePool
                GameObject poolObj = new GameObject("ProjectilePool");
                projectilePool = poolObj.AddComponent<Projectiles.ProjectilePool>();

                // ПУБЛИЧНЫЕ поля - назначаем напрямую!
                projectilePool.projectilePrefab = projectilePrefab;
                projectilePool.initialPoolSize = 20;

                // Вызвать Initialize() вручную, т.к. Awake может не сработать
                projectilePool.Initialize();
            }
        }

        /// <summary>
        /// Попытка выстрела
        /// </summary>
        private void TryShoot()
        {
            // Проверить кулдаун
            if (!cooldownTimer.IsReady)
            {
                return;
            }

            // Проверить ману
            if (!manaComponent.TrySpend(manaCost))
            {
                return;
            }

            // Выстрелить
            Shoot();

            // Запустить кулдаун
            cooldownTimer.Start(cooldown);
        }

        /// <summary>
        /// Выстрел
        /// </summary>
        private void Shoot()
        {
            if (projectilePool == null)
            {
                Debug.LogWarning("[SimpleShooter] ProjectilePool не создан!");
                return;
            }

            // Создать контекст каста (упрощённый, без бафов)
            CastContext context = new CastContext(
                caster: this,
                sourceSpell: null,
                damage: baseDamage,
                manaCost: manaCost,
                cooldown: cooldown,
                projectileSpeed: projectileSpeed,
                range: range,
                pierce: 0
            );

            // Получить снаряд из пула
            var projectile = projectilePool.Get(context);

            if (projectile == null)
            {
                Debug.LogError("[SimpleShooter] Не удалось создать снаряд!");
                return;
            }

            shotsFired++;
        }

        // ICaster implementation
        public bool TrySpendMana(float amount)
        {
            return manaComponent.TrySpend(amount);
        }

        public void OnSpellCasted(CastContext context)
        {
            // Пока ничего не делаем
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;

            GUI.Label(new Rect(10, 120, 300, 20), $"Shots Fired: {shotsFired}");
            GUI.Label(new Rect(10, 140, 300, 20), 
                $"Cooldown: {(cooldownTimer.IsReady ? "Ready" : cooldownTimer.Remaining.ToString("F1"))}");
        }
    }
}
