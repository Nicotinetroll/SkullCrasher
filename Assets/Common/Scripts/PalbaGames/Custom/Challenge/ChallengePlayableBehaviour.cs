using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;
using OctoberStudio;

namespace PalbaGames.Timeline
{
    public class ChallengePlayableBehaviour : PlayableBehaviour
    {
        public ChallengeType challengeType;
        public int amount;
        public float duration;
        public string description;

        // Particle settings
        public bool enableProgressParticles;
        public GameObject particlePrefab;
        public float particleIntensity = 1.0f;

        public bool enableStatBuffRewards;
        public StatModifierEntry[] rewardStatBuffs;

        public bool enableDropReward;
        public DropType rewardDropType;
        public int rewardDropCount;

        public bool enableStatDebuffPenalties;
        public StatModifierEntry[] penaltyStatDebuffs;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;

            System.Action onSuccess = () =>
            {
                // Apply stat buff rewards if enabled
                if (enableStatBuffRewards && StatBuffManager.Instance != null && rewardStatBuffs.Length > 0)
                {
                    StatBuffManager.Instance.ApplyStatBuffs(rewardStatBuffs);
                }

                // Apply drop reward if enabled
                if (enableDropReward && rewardDropCount > 0)
                {
                    Vector2 playerPos = new Vector2(
                        PlayerBehavior.Player.transform.position.x,
                        PlayerBehavior.Player.transform.position.y
                    );
                    RewardSystem.ApplyDropAtPosition(rewardDropType, playerPos, rewardDropCount, 2f);
                }
            };

            System.Action onFail = () =>
            {
                // Apply stat debuff penalties if enabled
                if (enableStatDebuffPenalties && StatPenaltyManager.Instance != null && penaltyStatDebuffs.Length > 0)
                {
                    StatPenaltyManager.Instance.ApplyStatPenalties(penaltyStatDebuffs);
                }
            };

            BaseChallenge challenge = challengeType switch
            {
                ChallengeType.KillEnemiesInTime => new KillEnemiesInTimeChallenge(amount, duration, onSuccess, onFail),
                ChallengeType.DealDamageInTime => new DealDamageInTimeChallenge(amount, duration, onSuccess, onFail),
                ChallengeType.DontMoveForTime => new DontMoveForTimeChallenge(duration, onSuccess, onFail),
                ChallengeType.DontTakeDamage => new DontTakeDamageChallenge(duration, onSuccess, onFail),
                ChallengeType.Survive => new SurviveForTimeChallenge(duration, onSuccess, onFail),
                _ => null
            };

            if (challenge != null)
            {
                string finalDescription = !string.IsNullOrEmpty(description) ? description :
                    challengeType switch
                    {
                        ChallengeType.KillEnemiesInTime => $"Kill {amount} enemies in {duration} seconds",
                        ChallengeType.DealDamageInTime => $"Deal {amount} damage in {duration} seconds",
                        ChallengeType.DontMoveForTime => $"Don't move for {duration} seconds",
                        ChallengeType.DontTakeDamage => $"Don't take damage for {duration} seconds",
                        ChallengeType.Survive => $"Survive for {duration} seconds",
                        _ => challenge.GetDisplayName()
                    };

                // Pass particle settings to ChallengeManager
                ChallengeManager.Instance.AddChallenge(challenge, finalDescription, enableProgressParticles, particlePrefab, particleIntensity);
            }
        }
    }
}