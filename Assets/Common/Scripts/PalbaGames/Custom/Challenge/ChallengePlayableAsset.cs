using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;
using OctoberStudio;

namespace PalbaGames.Timeline
{
    [System.Serializable]
    public class ChallengePlayableAsset : PlayableAsset
    {
        [Header("Challenge Settings")]
        [Tooltip("Type of challenge to trigger during timeline playback.")]
        public ChallengeType challengeType;

        [Tooltip("Main value parameter (e.g. kill count, damage amount, etc.). Used only for relevant challenge types.")]
        public int amount = 10;

        [Tooltip("Duration of the challenge in seconds.")]
        public float duration = 5f;

        [Tooltip("Custom description text for the challenge intro. If empty, auto-generated description will be used.")]
        public string description = "";

        [Header("Particle Effects")]
        [Space(5)]
        [Tooltip("Enable particle effects when progress bar fills during this challenge.")]
        public bool enableProgressParticles = false;

        [Tooltip("Particle prefab to spawn (drag your custom particle prefab here).")]
        public GameObject particlePrefab;

        [Tooltip("Particle effect intensity (0.1 = subtle, 1.0 = normal, 2.0 = intense).")]
        [Range(0.1f, 3.0f)]
        public float particleIntensity = 1.0f;

        [Header("Rewards (Success)")]
        [Space(5)]
        [Tooltip("Enable stat buff rewards when challenge succeeds.\n\n" +
                 "STAT MODIFIER MODES:\n" +
                 "• Multiplicative: current × value (1.5 = +50%, 0.5 = -50%)\n" +
                 "• Additive: current + value (+10 = +10% crit chance)\n" +
                 "• Absolute: set exact value (0 = disable completely)")]
        public bool enableStatBuffRewards = false;

        [Tooltip("List of stat buffs to apply on challenge success.\n\n" +
                 "EXAMPLES:\n" +
                 "• DamageMultiplier: 2.0, Multiplicative = double damage\n" +
                 "• CriticalChance: 15, Additive = +15% crit chance\n" +
                 "• SpeedMultiplier: 0, Absolute = disable movement")]
        public StatModifierEntry[] rewardStatBuffs = new StatModifierEntry[0];

        [Space(5)]
        [Tooltip("Enable drop reward when challenge succeeds.")]
        public bool enableDropReward = false;

        [Tooltip("DropType to spawn when challenge succeeds.")]
        public DropType rewardDropType;

        [Tooltip("How many items to drop.")]
        public int rewardDropCount = 3;

        [Header("Penalties (Failure)")]
        [Space(5)]
        [Tooltip("Enable stat debuff penalties when challenge fails.\n\n" +
                 "COMMON PENALTY EXAMPLES:\n" +
                 "• DamageMultiplier: 0, Absolute = disable damage completely\n" +
                 "• SpeedMultiplier: 0.5, Multiplicative = half speed\n" +
                 "• CriticalChance: 0, Absolute = disable crits")]
        public bool enableStatDebuffPenalties = false;

        [Tooltip("List of stat debuffs to apply on challenge failure.\n\n" +
                 "TIP: Use Absolute mode for harsh penalties (disable stats)\n" +
                 "Use Multiplicative for percentage reductions")]
        public StatModifierEntry[] penaltyStatDebuffs = new StatModifierEntry[0];

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChallengePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.challengeType = challengeType;
            behaviour.amount = amount;
            behaviour.duration = duration;
            behaviour.description = description;

            // Particle settings
            behaviour.enableProgressParticles = enableProgressParticles;
            behaviour.particlePrefab = particlePrefab;
            behaviour.particleIntensity = particleIntensity;

            behaviour.enableStatBuffRewards = enableStatBuffRewards;
            behaviour.rewardStatBuffs = rewardStatBuffs;

            behaviour.enableDropReward = enableDropReward;
            behaviour.rewardDropType = rewardDropType;
            behaviour.rewardDropCount = rewardDropCount;

            behaviour.enableStatDebuffPenalties = enableStatDebuffPenalties;
            behaviour.penaltyStatDebuffs = penaltyStatDebuffs;

            return playable;
        }
    }
}