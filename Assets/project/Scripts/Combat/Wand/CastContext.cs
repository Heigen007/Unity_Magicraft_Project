using UnityEngine;

namespace Magicraft.Combat
{
    /// <summary>
    /// Контекст каста заклинания с финальными параметрами после применения всех модификаторов
    /// Содержит всю информацию, необходимую для исполнения заклинания
    /// </summary>
    public class CastContext
    {
        // Ссылки на источники
        public ICaster Caster { get; private set; }
        public SpellSO SourceSpell { get; private set; }

        // Финальные параметры урона
        public float Damage { get; private set; }
        public float CritChance { get; private set; }
        public float CritMultiplier { get; private set; }

        // Финальные параметры ресурсов
        public float ManaCost { get; private set; }
        public float Cooldown { get; private set; }

        // Финальные параметры снаряда
        public float ProjectileSpeed { get; private set; }
        public float Range { get; private set; }
        public int Pierce { get; private set; }

        // Позиция и направление
        public Vector2 SpawnPosition { get; private set; }
        public Vector2 Direction { get; private set; }

        /// <summary>
        /// Конструктор контекста каста
        /// </summary>
        public CastContext(
            ICaster caster,
            SpellSO sourceSpell,
            float damage,
            float manaCost,
            float cooldown,
            float projectileSpeed,
            float range,
            int pierce,
            float critChance = 0f,
            float critMultiplier = 2f)
        {
            Caster = caster;
            SourceSpell = sourceSpell;
            Damage = damage;
            ManaCost = manaCost;
            Cooldown = cooldown;
            ProjectileSpeed = projectileSpeed;
            Range = range;
            Pierce = pierce;
            CritChance = critChance;
            CritMultiplier = critMultiplier;

            SpawnPosition = caster.Muzzle.position;
            Direction = caster.AimDirection;
        }

        /// <summary>
        /// Билдер для удобного создания контекста с модификаторами
        /// </summary>
        public class Builder
        {
            private ICaster caster;
            private SpellSO sourceSpell;
            
            private float damageMultiplier = 1f;
            private float manaCostMultiplier = 1f;
            private float cooldownMultiplier = 1f;
            private float speedMultiplier = 1f;
            private int pierceAdditive = 0;
            private float critChanceAdditive = 0f;
            private float critMultiplier = 2f;

            public Builder(ICaster caster, SpellSO spell)
            {
                this.caster = caster;
                this.sourceSpell = spell;
            }

            public Builder ApplyDamageMultiplier(float mult) 
            { 
                damageMultiplier *= mult; 
                return this; 
            }

            public Builder ApplyManaCostMultiplier(float mult) 
            { 
                manaCostMultiplier *= mult; 
                return this; 
            }

            public Builder ApplyCooldownMultiplier(float mult) 
            { 
                cooldownMultiplier *= mult; 
                return this; 
            }

            public Builder ApplySpeedMultiplier(float mult) 
            { 
                speedMultiplier *= mult; 
                return this; 
            }

            public Builder AddPierce(int value) 
            { 
                pierceAdditive += value; 
                return this; 
            }

            public Builder AddCritChance(float value) 
            { 
                critChanceAdditive += value; 
                return this; 
            }

            public Builder SetCritMultiplier(float mult) 
            { 
                critMultiplier = mult; 
                return this; 
            }

            public CastContext Build()
            {
                float finalDamage = sourceSpell.BaseDamage * damageMultiplier;
                float finalManaCost = sourceSpell.BaseManaCost * manaCostMultiplier;
                float finalCooldown = sourceSpell.BaseCooldown * cooldownMultiplier;
                float finalSpeed = sourceSpell.ProjectileSpeed * speedMultiplier;
                float finalRange = sourceSpell.Range;
                int finalPierce = sourceSpell.Pierce + pierceAdditive;

                return new CastContext(
                    caster,
                    sourceSpell,
                    finalDamage,
                    finalManaCost,
                    finalCooldown,
                    finalSpeed,
                    finalRange,
                    finalPierce,
                    critChanceAdditive,
                    critMultiplier
                );
            }
        }
    }
}
