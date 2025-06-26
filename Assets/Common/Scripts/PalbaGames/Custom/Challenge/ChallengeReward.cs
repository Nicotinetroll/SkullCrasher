using OctoberStudio;
using UnityEngine;

namespace PalbaGames.Challenges
{
    public enum RewardLogicType
    {
        None,
        Buff,
        Heal,
        Drop
    }

    [System.Serializable]
    public struct ChallengeReward
    {
        public bool enabled;

        public RewardLogicType logicType;

        public BuffType buffType;
        public float buffDuration;

        public int healAmount;

        public DropType dropType;
        public int dropCount;
    }
}