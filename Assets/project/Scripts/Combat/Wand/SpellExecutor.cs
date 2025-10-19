using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Исполнитель заклинаний
    /// Отвечает за выполнение заклинания на основе CastContext
    /// </summary>
    public class SpellExecutor
    {
        /// <summary>
        /// Исполнить заклинание
        /// </summary>
        /// <param name="context">Контекст каста с финальными параметрами</param>
        public void Execute(CastContext context)
        {
            if (context == null)
            {
                Debug.LogError("[SpellExecutor] CastContext is null!");
                return;
            }

            if (context.SourceSpell == null)
            {
                Debug.LogError("[SpellExecutor] SourceSpell is null!");
                return;
            }

            // Выбор типа исполнения на основе типа заклинания
            switch (context.SourceSpell.ExecutionType)
            {
                case SpellExecutionType.Projectile:
                    ExecuteProjectile(context);
                    break;

                case SpellExecutionType.Beam:
                    Debug.LogWarning("[SpellExecutor] Beam spells not yet implemented!");
                    break;

                case SpellExecutionType.AoE:
                    Debug.LogWarning("[SpellExecutor] AoE spells not yet implemented!");
                    break;

                case SpellExecutionType.Self:
                    Debug.LogWarning("[SpellExecutor] Self spells not yet implemented!");
                    break;

                default:
                    Debug.LogError($"[SpellExecutor] Unknown execution type: {context.SourceSpell.ExecutionType}");
                    break;
            }
        }

        /// <summary>
        /// Исполнить заклинание типа Projectile
        /// </summary>
        private void ExecuteProjectile(CastContext context)
        {
            // Получить префаб снаряда
            GameObject prefab = context.SourceSpell.ProjectilePrefab;

            // Если префаб не указан в SpellSO, используем базовый из пула
            if (prefab == null)
            {
                // Использовать ProjectilePool для создания снаряда
                var pool = Projectiles.ProjectilePool.Instance;
                if (pool != null)
                {
                    // ProjectilePool.Get() уже принимает context и инициализирует снаряд
                    var projectile = pool.Get(context);
                    if (projectile == null)
                    {
                        Debug.LogError("[SpellExecutor] Failed to get projectile from pool!");
                    }
                }
                else
                {
                    Debug.LogError("[SpellExecutor] ProjectilePool.Instance is null!");
                }
            }
            else
            {
                // Использовать кастомный префаб из SpellSO
                Debug.LogWarning("[SpellExecutor] Custom projectile prefabs not yet fully supported!");
                
                // TODO: Создать систему пулов для разных типов снарядов
                GameObject projectileObj = Object.Instantiate(prefab, context.SpawnPosition, Quaternion.identity);
                var projectile = projectileObj.GetComponent<Projectiles.Projectile>();
                
                if (projectile != null)
                {
                    projectile.Initialize(context, (p) => Object.Destroy(p.gameObject));
                }
            }
        }

        /// <summary>
        /// Исполнить заклинание типа Beam (будущее)
        /// </summary>
        private void ExecuteBeam(CastContext context)
        {
            // TODO: Реализация лучевых заклинаний
            Debug.LogWarning("[SpellExecutor] Beam execution not implemented yet!");
        }

        /// <summary>
        /// Исполнить заклинание типа AoE (будущее)
        /// </summary>
        private void ExecuteAoE(CastContext context)
        {
            // TODO: Реализация АОЕ заклинаний
            Debug.LogWarning("[SpellExecutor] AoE execution not implemented yet!");
        }

        /// <summary>
        /// Исполнить заклинание типа Self (будущее)
        /// </summary>
        private void ExecuteSelf(CastContext context)
        {
            // TODO: Реализация бафов на себя
            Debug.LogWarning("[SpellExecutor] Self execution not implemented yet!");
        }
    }
}
