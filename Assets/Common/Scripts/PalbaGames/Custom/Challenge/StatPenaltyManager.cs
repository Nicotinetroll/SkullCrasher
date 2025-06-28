using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Advanced penalty system that can modify any player stat for a duration.
    /// Works with PlayerStatModifier to apply temporary debuffs.
    /// </summary>
    public class StatPenaltyManager : MonoBehaviour
    {
        public static StatPenaltyManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        private PlayerStatModifier playerModifier;
        private Dictionary<PlayerStatType, float> originalValues = new();
        private Dictionary<PlayerStatType, Coroutine> activeCoroutines = new();

        /// <summary>
        /// Currently active stat penalties with remaining time.
        /// </summary>
        public Dictionary<PlayerStatType, float> ActivePenalties { get; private set; } = new();

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
                Debug.LogError("[StatPenaltyManager] PlayerStatModifier not found!");
                return;
            }

            // Store original values
            StoreOriginalValues();
        }

        /// <summary>
        /// Apply a stat penalty for specified duration.
        /// </summary>
        public void ApplyStatPenalty(PlayerStatType statType, float penaltyValue, float duration, StatModifierMode mode = StatModifierMode.Multiplicative)
        {
            if (playerModifier == null) return;

            // Cancel existing penalty for this stat
            if (activeCoroutines.ContainsKey(statType))
            {
                StopCoroutine(activeCoroutines[statType]);
                RemoveStatPenalty(statType);
            }

            // Store original value if not already stored
            if (!originalValues.ContainsKey(statType))
            {
                originalValues[statType] = GetCurrentStatValue(statType);
            }

            // Apply penalty
            SetStatValue(statType, penaltyValue);
            ActivePenalties[statType] = duration;

            if (showDebugLogs)
                Debug.Log($"[StatPenaltyManager] Applied {statType} penalty: {penaltyValue} for {duration}s");

            // Start countdown coroutine
            activeCoroutines[statType] = StartCoroutine(RemoveStatPenaltyAfterTime(statType, duration));
        }

        private IEnumerator RemoveStatPenaltyAfterTime(PlayerStatType statType, float duration)
        {
            while (ActivePenalties.ContainsKey(statType) && ActivePenalties[statType] > 0f)
            {
                ActivePenalties[statType] -= Time.deltaTime;
                yield return null;
            }

            RemoveStatPenalty(statType);
        }

        /// <summary>
        /// Apply multiple stat penalties from list.
        /// </summary>
        public void ApplyStatPenalties(StatModifierEntry[] penaltyEntries)
        {
            foreach (var entry in penaltyEntries)
            {
                ApplyStatPenalty(entry.statType, entry.value, entry.duration, entry.mode);
            }
        }

        /// <summary>
        /// Manually remove a specific stat penalty.
        /// </summary>
        public void RemoveStatPenalty(PlayerStatType statType)
        {
            if (!originalValues.ContainsKey(statType)) return;

            // Restore original value
            SetStatValue(statType, originalValues[statType]);
            
            // Cleanup tracking
            originalValues.Remove(statType);
            ActivePenalties.Remove(statType);
            
            if (activeCoroutines.ContainsKey(statType))
            {
                activeCoroutines.Remove(statType);
            }

            if (showDebugLogs)
                Debug.Log($"[StatPenaltyManager] Removed {statType} penalty");
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
    }
}