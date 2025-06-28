using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Advanced buff system that can modify any player stat for a duration.
    /// Works with PlayerStatModifier to apply temporary buffs.
    /// </summary>
    public class StatBuffManager : MonoBehaviour
    {
        public static StatBuffManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private PlayerStatModifier playerModifier;
        private Dictionary<PlayerStatType, float> originalValues = new();
        private Dictionary<PlayerStatType, Coroutine> activeCoroutines = new();

        /// <summary>
        /// Currently active stat buffs with remaining time.
        /// </summary>
        public Dictionary<PlayerStatType, float> ActiveBuffs { get; private set; } = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Get or create PlayerStatModifier
            playerModifier = PlayerBehavior.Player?.GetComponent<PlayerStatModifier>();
            if (playerModifier == null && PlayerBehavior.Player != null)
            {
                playerModifier = PlayerBehavior.Player.gameObject.AddComponent<PlayerStatModifier>();
            }

            if (playerModifier == null)
            {
                Debug.LogError("[StatBuffManager] PlayerStatModifier not found!");
                return;
            }

            // Store original values
            StoreOriginalValues();
        }

        /// <summary>
        /// Apply a stat buff for specified duration.
        /// </summary>
        public void ApplyStatBuff(PlayerStatType statType, float buffValue, float duration, StatModifierMode mode = StatModifierMode.Multiplicative)
        {
            if (playerModifier == null) return;

            // Cancel existing buff for this stat
            if (activeCoroutines.ContainsKey(statType))
            {
                StopCoroutine(activeCoroutines[statType]);
                RemoveStatBuff(statType);
            }

            // Store original value if not already stored
            if (!originalValues.ContainsKey(statType))
            {
                originalValues[statType] = GetCurrentStatValue(statType);
            }

            // Calculate final value based on mode
            float currentValue = originalValues[statType];
            float finalValue = mode switch
            {
                StatModifierMode.Multiplicative => currentValue * buffValue,
                StatModifierMode.Additive => currentValue + buffValue,
                StatModifierMode.Absolute => buffValue,
                _ => buffValue
            };

            // Apply buff
            SetStatValue(statType, finalValue);
            ActiveBuffs[statType] = duration;

            if (showDebugLogs)
                Debug.Log($"[StatBuffManager] Applied {statType} buff: {mode} {buffValue} = {finalValue} for {duration}s");

            // Start countdown coroutine
            activeCoroutines[statType] = StartCoroutine(RemoveStatBuffAfterTime(statType, duration));
        }

        /// <summary>
        /// Apply multiple stat buffs from list.
        /// </summary>
        public void ApplyStatBuffs(StatModifierEntry[] buffEntries)
        {
            foreach (var entry in buffEntries)
            {
                ApplyStatBuff(entry.statType, entry.value, entry.duration, entry.mode);
            }
        }

        /// <summary>
        /// Apply multiple stat buffs simultaneously.
        /// </summary>
        public void ApplyMultipleStatBuffs(Dictionary<PlayerStatType, float> buffs, float duration)
        {
            foreach (var kvp in buffs)
            {
                ApplyStatBuff(kvp.Key, kvp.Value, duration);
            }
        }

        private IEnumerator RemoveStatBuffAfterTime(PlayerStatType statType, float duration)
        {
            while (ActiveBuffs.ContainsKey(statType) && ActiveBuffs[statType] > 0f)
            {
                ActiveBuffs[statType] -= Time.deltaTime;
                yield return null;
            }

            RemoveStatBuff(statType);
        }

        /// <summary>
        /// Manually remove a specific stat buff.
        /// </summary>
        public void RemoveStatBuff(PlayerStatType statType)
        {
            if (!originalValues.ContainsKey(statType)) return;

            // Restore original value
            SetStatValue(statType, originalValues[statType]);
            
            // Cleanup tracking
            originalValues.Remove(statType);
            ActiveBuffs.Remove(statType);
            
            if (activeCoroutines.ContainsKey(statType))
            {
                activeCoroutines.Remove(statType);
            }

            if (showDebugLogs)
                Debug.Log($"[StatBuffManager] Removed {statType} buff");
        }

        /// <summary>
        /// Remove all active buffs.
        /// </summary>
        public void RemoveAllBuffs()
        {
            var statsToRemove = new List<PlayerStatType>(ActiveBuffs.Keys);
            foreach (var stat in statsToRemove)
            {
                RemoveStatBuff(stat);
            }
        }

        private void StoreOriginalValues()
        {
            if (playerModifier == null) return;

            originalValues[PlayerStatType.DamageMultiplier] = playerModifier.damageMultiplier;
            originalValues[PlayerStatType.SpeedMultiplier] = playerModifier.speedMultiplier;
            originalValues[PlayerStatType.MagnetRadiusMultiplier] = playerModifier.magnetRadiusMultiplier;
            originalValues[PlayerStatType.XpMultiplier] = playerModifier.xpMultiplier;
            originalValues[PlayerStatType.CooldownMultiplier] = playerModifier.cooldownMultiplier;
            originalValues[PlayerStatType.DamageReductionExtraPercent] = playerModifier.damageReductionExtraPercent;
            originalValues[PlayerStatType.ProjectileSpeedMultiplier] = playerModifier.projectileSpeedMultiplier;
            originalValues[PlayerStatType.SizeMultiplier] = playerModifier.sizeMultiplier;
            originalValues[PlayerStatType.DurationMultiplier] = playerModifier.durationMultiplier;
            originalValues[PlayerStatType.GoldMultiplier] = playerModifier.goldMultiplier;
            
            // Store critical strike values
            var critExtended = PlayerBehavior.Player?.GetComponent<PlayerBehavior_Extended>();
            if (critExtended != null)
            {
                originalValues[PlayerStatType.CriticalChance] = critExtended.criticalChance;
                originalValues[PlayerStatType.CriticalMultiplierMin] = critExtended.criticalMultiplierMin;
                originalValues[PlayerStatType.CriticalMultiplierMax] = critExtended.criticalMultiplierMax;
            }
        }

        private float GetCurrentStatValue(PlayerStatType statType)
        {
            if (playerModifier == null) return 1f;

            return playerModifier.GetStatValue(statType);
        }

        private void SetStatValue(PlayerStatType statType, float value)
        {
            if (playerModifier == null) return;

            playerModifier.SetStatValue(statType, value);

            // Apply changes immediately
            playerModifier.ApplyMultipliers();
        }

        /// <summary>
        /// Get buff description for UI.
        /// </summary>
        public string GetActiveBuffsDescription()
        {
            if (ActiveBuffs.Count == 0) return "";

            string description = "Active Buffs:\n";
            foreach (var kvp in ActiveBuffs)
            {
                description += $"{kvp.Key}: {kvp.Value:F1}s\n";
            }
            return description;
        }
    }
}