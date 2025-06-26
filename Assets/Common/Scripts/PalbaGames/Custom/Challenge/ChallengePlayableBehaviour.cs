using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;

namespace PalbaGames.Timeline
{
    public class ChallengePlayableBehaviour : PlayableBehaviour
    {
        public ChallengeType challengeType;
        public int amount;
        public float duration;

        public RewardType reward;
        public PenaltyType penalty;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (!Application.isPlaying) return;

            System.Action rewardCallback = () => RewardSystem.Apply(reward);
            System.Action penaltyCallback = () => RewardSystem.ApplyPenalty(penalty);

            BaseChallenge challenge = challengeType switch
            {
                ChallengeType.KillEnemiesInTime => new KillXEnemiesInYSecondsChallenge(amount, duration, rewardCallback, penaltyCallback),
                // ďalšie typy sem
                _ => null
            };

            if (challenge != null)
                ChallengeManager.Instance.AddChallenge(challenge);
        }
    }
}