using UnityEngine;
using System.Collections.Generic;

namespace Magicraft.Combat
{
    /// <summary>
    /// Компонент врага для нанесения урона игроку при касании
    /// Наносит урон каждые X секунд при контакте
    /// КАЖДЫЙ ВРАГ имеет свой таймер для каждой цели
    /// </summary>
    public class EnemyDamageOnContact : MonoBehaviour
    {
        [Header("Damage Settings")]
        [Tooltip("Урон за одно касание")]
        [SerializeField] private float damagePerHit = 5f;

        [Tooltip("Интервал между ударами (секунды)")]
        [SerializeField] private float damageInterval = 0.5f;

        [Tooltip("Слой игрока")]
        [SerializeField] private LayerMask playerLayer;

        // Таймеры урона для КАЖДОГО врага отдельно (Dictionary: цель -> таймер)
        private Dictionary<IDamageable, float> damageTimers = new Dictionary<IDamageable, float>();

        private void Update()
        {
            // Обновление всех таймеров
            List<IDamageable> toRemove = new List<IDamageable>();
            List<IDamageable> keys = new List<IDamageable>(damageTimers.Keys);
            
            foreach (var target in keys)
            {
                if (target == null || !target.IsAlive)
                {
                    toRemove.Add(target);
                    continue;
                }
                
                float timer = damageTimers[target];
                timer -= Time.deltaTime;
                damageTimers[target] = timer;
            }
            
            // Удаляем мертвые цели
            foreach (var target in toRemove)
            {
                damageTimers.Remove(target);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Проверяем, что это игрок
            if (((1 << collision.gameObject.layer) & playerLayer) == 0)
            {
                return;
            }

            // Получаем компонент урона
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable == null || !damageable.IsAlive)
            {
                return;
            }

            // Получить или создать таймер для этой цели
            if (!damageTimers.ContainsKey(damageable))
            {
                damageTimers[damageable] = 0f;
            }

            // Если таймер истек - наносим урон
            if (damageTimers[damageable] <= 0f)
            {
                damageable.ApplyDamage(damagePerHit, gameObject);
                damageTimers[damageable] = damageInterval;
                
                Debug.Log($"[Enemy {gameObject.name}] Dealt {damagePerHit} damage to {collision.name}");
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Сброс таймера при выходе из триггера
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageTimers.ContainsKey(damageable))
            {
                damageTimers.Remove(damageable);
            }
        }
    }
}
