using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;
using OctoberStudio;

namespace PalbaGames.Timeline
{
    [System.Serializable]
    public class ChallengePlayableAsset : PlayableAsset
    {
        [Tooltip("Type of challenge to trigger during timeline playback.")]
        public ChallengeType challengeType;

        [Tooltip("Main value parameter (e.g. kill count, damage amount, etc.). Used only for relevant challenge types.")]
        public int amount = 10;

        [Tooltip("Duration of the challenge in seconds.")]
        public float duration = 5f;

        [Header("Reward")]
        [Tooltip("Main reward applied when the challenge is completed successfully.")]
        public RewardType reward;

        [Tooltip("Enable to also drop a reward item when the challenge is completed.")]
        public bool enableDropReward;

        [Tooltip("DropType to spawn if drop reward is enabled.")]
        public DropType dropReward;

        [Tooltip("How many items to drop.")]
        public int dropCount = 0;

        [Header("Penalty")]
        [Tooltip("Penalty applied if the challenge fails.")]
        public PenaltyType penalty;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChallengePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.challengeType = challengeType;
            behaviour.amount = amount;
            behaviour.duration = duration;

            behaviour.reward = reward;
            behaviour.enableDropReward = enableDropReward;
            behaviour.dropReward = dropReward;
            behaviour.dropCount = dropCount;

            behaviour.penalty = penalty;

            return playable;
        }
    }
}