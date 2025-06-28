using UnityEngine;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Single stat modifier entry for lists in Timeline.
    /// </summary>
    [System.Serializable]
    public class StatModifierEntry
    {
        [Tooltip("Player stat to modify.")]
        public PlayerStatType statType = PlayerStatType.DamageMultiplier;

        [Tooltip("Modifier value:\n" +
                 "• Multiplicative: current × value (e.g., 1.5 = +50%, 0.5 = -50%)\n" +
                 "• Additive: current + value (e.g., +10 crit chance, +2.0 damage)\n" +
                 "• Absolute: set to exact value (e.g., 0 = disable completely)")]
        public float value = 1.5f;

        [Tooltip("How to apply the modifier:\n" +
                 "• Multiplicative: Multiply current value (1.5 = +50% boost)\n" +
                 "• Additive: Add to current value (+10 = +10% crit chance)\n" +
                 "• Absolute: Set to exact value (0 = disable stat completely)")]
        public StatModifierMode mode = StatModifierMode.Multiplicative;

        [Tooltip("Duration of this modifier in seconds.")]
        public float duration = 10f;
    }

    /// <summary>
    /// How the stat modifier value should be applied.
    /// </summary>
    public enum StatModifierMode
    {
        /// <summary>Current value × modifier (e.g., damage × 1.5 = +50% damage)</summary>
        [Tooltip("Multiply: current × value\nExample: damage 100 × 1.5 = 150 (+50%)")]
        Multiplicative,
        
        /// <summary>Current value + modifier (e.g., damage + 2.0 = +2.0 damage)</summary>
        [Tooltip("Add: current + value\nExample: crit chance 10% + 5 = 15%")]
        Additive,
        
        /// <summary>Set to exact value (e.g., damage = 0 for disable)</summary>
        [Tooltip("Set: value exactly\nExample: damage = 0 (disable completely)")]
        Absolute
    }
}