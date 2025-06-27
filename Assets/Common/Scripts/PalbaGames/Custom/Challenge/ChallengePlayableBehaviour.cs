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

        public RewardType reward;
        public bool enableDropReward;
        public DropType dropReward;
        public int dropCount;

        public PenaltyType penalty;

        public string description; // 🟢 Added

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
                // TODO: add other types
                _ => null
            };

            if (challenge != null)
            {
                ChallengeManager.Instance.AddChallenge(challenge, description); // 🟢 Add description here
            }
        }
    }
}