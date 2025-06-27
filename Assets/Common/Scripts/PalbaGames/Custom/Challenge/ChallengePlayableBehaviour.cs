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
        public string description; // Add this line

        public RewardType reward;
        public bool enableDropReward;
        public DropType dropReward;
        public int dropCount;

        public PenaltyType penalty;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;

            System.Action onSuccess = () =>
            {
                RewardSystem.Apply(reward);

                if (enableDropReward && dropCount > 0)
                {
                    RewardSystem.ApplyDrop(dropReward, dropCount);
                }
            };

            System.Action onFail = () =>
            {
                RewardSystem.ApplyPenalty(penalty);
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
                // Use custom description if provided, otherwise generate one
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

                // Let ChallengeManager handle the entire UI sequence
                ChallengeManager.Instance.AddChallenge(challenge, finalDescription);
            }
        }
    }
}