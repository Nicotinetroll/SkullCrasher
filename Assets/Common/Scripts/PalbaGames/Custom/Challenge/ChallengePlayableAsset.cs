using UnityEngine;
using UnityEngine.Playables;
using PalbaGames.Challenges;

namespace PalbaGames.Timeline
{
    [System.Serializable]
    public class ChallengePlayableAsset : PlayableAsset
    {
        public ChallengeType challengeType;
        public int amount = 10;
        public float duration = 5f;

        public RewardType reward = RewardType.BuffCritChance;
        public PenaltyType penalty = PenaltyType.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChallengePlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.challengeType = challengeType;
            behaviour.amount = amount;
            behaviour.duration = duration;
            behaviour.reward = reward;
            behaviour.penalty = penalty;

            return playable;
        }
    }
}