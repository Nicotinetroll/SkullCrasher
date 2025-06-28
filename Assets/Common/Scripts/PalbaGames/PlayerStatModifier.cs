using UnityEngine;
using OctoberStudio;
using PalbaGames.Challenges; // Add this line

namespace PalbaGames
{
    /// <summary>
    /// Allows full runtime modification of PlayerBehavior stats via multipliers and base overrides.
    /// Also handles critical strike modifiers.
    /// Attach to same GameObject as PlayerBehavior.
    /// </summary>
    public class PlayerStatModifier : MonoBehaviour
    {
        [Header("Multipliers (x1 = default)")]
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float magnetRadiusMultiplier = 1f;
        public float xpMultiplier = 1f;
        public float cooldownMultiplier = 1f;
        public float damageReductionExtraPercent = 0f;
        public float projectileSpeedMultiplier = 1f;
        public float sizeMultiplier = 1f;
        public float durationMultiplier = 1f;
        public float goldMultiplier = 1f;

        [Header("Critical Strike Modifiers")]
        public float criticalChanceModifier = 0f; // Additive (e.g., +5 = +5% crit chance)
        public float criticalMultiplierMinModifier = 0f; // Additive (e.g., +0.5 = +0.5x min crit)
        public float criticalMultiplierMaxModifier = 0f; // Additive (e.g., +1.0 = +1.0x max crit)

        [Header("Base Stat Overrides (null = keep original)")]
        public float? overrideBaseDamage;
        public float? overrideBaseSpeed;
        public float? overrideBaseHP;
        public float? overrideBaseMagnetRadius;
        public float? overrideBaseXPMultiplier;
        public float? overrideBaseCooldownMultiplier;
        public float? overrideBaseDamageReductionPercent;
        public float? overrideBaseProjectileSpeedMultiplier;
        public float? overrideBaseSizeMultiplier;
        public float? overrideBaseDurationMultiplier;
        public float? overrideBaseGoldMultiplier;

        private PlayerBehavior player;
        private PlayerBehavior_Extended critExtended;

        private void Awake()
        {
            player = PlayerBehavior.Player ?? FindObjectOfType<PlayerBehavior>();
            critExtended = GetComponent<PlayerBehavior_Extended>();
            
            if (player == null)
                Debug.LogError("[PlayerStatModifier] PlayerBehavior not found.");
        }

        private void Start()
        {
            ApplyBaseOverrides();
            ApplyMultipliers();
        }

        /// <summary>
        /// Apply all base overrides to PlayerBehavior (if set).
        /// </summary>
        public void ApplyBaseOverrides()
        {
            if (player == null) return;

            if (overrideBaseDamage.HasValue) player.SetBaseDamage(overrideBaseDamage.Value);
            if (overrideBaseSpeed.HasValue) player.SetBaseSpeed(overrideBaseSpeed.Value);
            if (overrideBaseHP.HasValue) player.SetBaseHP(overrideBaseHP.Value);
            if (overrideBaseMagnetRadius.HasValue) player.SetBaseMagnetRadius(overrideBaseMagnetRadius.Value);
            if (overrideBaseXPMultiplier.HasValue) player.SetBaseXPMultiplier(overrideBaseXPMultiplier.Value);
            if (overrideBaseCooldownMultiplier.HasValue) player.SetBaseCooldownMultiplier(overrideBaseCooldownMultiplier.Value);
            if (overrideBaseDamageReductionPercent.HasValue) player.SetBaseDamageReductionPercent(overrideBaseDamageReductionPercent.Value);
            if (overrideBaseProjectileSpeedMultiplier.HasValue) player.SetBaseProjectileSpeedMultiplier(overrideBaseProjectileSpeedMultiplier.Value);
            if (overrideBaseSizeMultiplier.HasValue) player.SetBaseSizeMultiplier(overrideBaseSizeMultiplier.Value);
            if (overrideBaseDurationMultiplier.HasValue) player.SetBaseDurationMultiplier(overrideBaseDurationMultiplier.Value);
            if (overrideBaseGoldMultiplier.HasValue) player.SetBaseGoldMultiplier(overrideBaseGoldMultiplier.Value);
        }

        /// <summary>
        /// Apply all multipliers to recalculate runtime stats.
        /// </summary>
        public void ApplyMultipliers()
        {
            if (player == null) return;

            player.RecalculateDamage(damageMultiplier);
            player.RecalculateMoveSpeed(speedMultiplier);
            player.RecalculateMagnetRadius(magnetRadiusMultiplier);
            player.RecalculateXPMuliplier(xpMultiplier);
            player.RecalculateCooldownMuliplier(cooldownMultiplier);
            player.RecalculateDamageReduction(damageReductionExtraPercent);
            player.RecalculateProjectileSpeedMultiplier(projectileSpeedMultiplier);
            player.RecalculateSizeMultiplier(sizeMultiplier);
            player.RecalculateDurationMultiplier(durationMultiplier);
            player.RecalculateGoldMultiplier(goldMultiplier);

            // Apply critical strike modifiers
            ApplyCriticalModifiers();
        }

        /// <summary>
        /// Apply critical strike modifiers to PlayerBehavior_Extended.
        /// </summary>
        private void ApplyCriticalModifiers()
        {
            if (critExtended == null) return;

            // Note: These should be applied to base values + modifiers
            // For now, we'll directly modify the extended component
            // In a real system, you'd want base values stored and calculated properly
        }

        /// <summary>
        /// Get current value for a specific stat type.
        /// </summary>
        public float GetStatValue(PlayerStatType statType)
        {
            return statType switch
            {
                PlayerStatType.DamageMultiplier => damageMultiplier,
                PlayerStatType.SpeedMultiplier => speedMultiplier,
                PlayerStatType.MagnetRadiusMultiplier => magnetRadiusMultiplier,
                PlayerStatType.XpMultiplier => xpMultiplier,
                PlayerStatType.CooldownMultiplier => cooldownMultiplier,
                PlayerStatType.DamageReductionExtraPercent => damageReductionExtraPercent,
                PlayerStatType.ProjectileSpeedMultiplier => projectileSpeedMultiplier,
                PlayerStatType.SizeMultiplier => sizeMultiplier,
                PlayerStatType.DurationMultiplier => durationMultiplier,
                PlayerStatType.GoldMultiplier => goldMultiplier,
                PlayerStatType.CriticalChance => critExtended?.criticalChance ?? 0f,
                PlayerStatType.CriticalMultiplierMin => critExtended?.criticalMultiplierMin ?? 1f,
                PlayerStatType.CriticalMultiplierMax => critExtended?.criticalMultiplierMax ?? 1f,
                _ => 1f
            };
        }

        /// <summary>
        /// Set value for a specific stat type.
        /// </summary>
        public void SetStatValue(PlayerStatType statType, float value)
        {
            switch (statType)
            {
                case PlayerStatType.DamageMultiplier:
                    damageMultiplier = value;
                    break;
                case PlayerStatType.SpeedMultiplier:
                    speedMultiplier = value;
                    break;
                case PlayerStatType.MagnetRadiusMultiplier:
                    magnetRadiusMultiplier = value;
                    break;
                case PlayerStatType.XpMultiplier:
                    xpMultiplier = value;
                    break;
                case PlayerStatType.CooldownMultiplier:
                    cooldownMultiplier = value;
                    break;
                case PlayerStatType.DamageReductionExtraPercent:
                    damageReductionExtraPercent = value;
                    break;
                case PlayerStatType.ProjectileSpeedMultiplier:
                    projectileSpeedMultiplier = value;
                    break;
                case PlayerStatType.SizeMultiplier:
                    sizeMultiplier = value;
                    break;
                case PlayerStatType.DurationMultiplier:
                    durationMultiplier = value;
                    break;
                case PlayerStatType.GoldMultiplier:
                    goldMultiplier = value;
                    break;
                case PlayerStatType.CriticalChance:
                    if (critExtended != null) critExtended.criticalChance = value;
                    break;
                case PlayerStatType.CriticalMultiplierMin:
                    if (critExtended != null) critExtended.criticalMultiplierMin = value;
                    break;
                case PlayerStatType.CriticalMultiplierMax:
                    if (critExtended != null) critExtended.criticalMultiplierMax = value;
                    break;
            }
        }

        /// <summary>
        /// Fully re-apply both base overrides and multipliers.
        /// </summary>
        public void ApplyAll()
        {
            ApplyBaseOverrides();
            ApplyMultipliers();
        }
    }
}