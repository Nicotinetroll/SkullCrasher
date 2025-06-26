using UnityEngine;
using OctoberStudio;

namespace PalbaGames
{
    /// <summary>
    /// Allows full runtime modification of PlayerBehavior stats via multipliers and base overrides.
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

        private void Awake()
        {
            player = PlayerBehavior.Player ?? FindObjectOfType<PlayerBehavior>();
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
