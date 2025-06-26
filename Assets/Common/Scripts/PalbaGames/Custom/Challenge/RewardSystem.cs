using UnityEngine;
using PalbaGames.Challenges;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    public static class RewardSystem
    {
        public static void Apply(RewardType type)
        {
            var pos = new Vector2(
                PlayerBehavior.Player.transform.position.x,
                PlayerBehavior.Player.transform.position.y
            );

            switch (type)
            {
                case RewardType.BuffDamage:
                    BuffManager.Instance.ApplyBuff(BuffType.DamageBoost, 10f);
                    break;

                case RewardType.BuffCritChance:
                    BuffManager.Instance.ApplyBuff(BuffType.CritChance, 10f);
                    break;

                case RewardType.Heal:
                    PlayerBehavior.Player.Heal(50);
                    break;

                case RewardType.DropChest:
                    StageController.DropManager.Drop(DropType.Chest, pos);
                    break;

                case RewardType.None:
                default:
                    break;
            }

            Debug.Log($"[Reward] Applied: {type}");
        }

        public static void ApplyPenalty(PenaltyType type)
        {
            switch (type)
            {
                case PenaltyType.TakeDamage:
                    PlayerBehavior.Player.TakeDamage(50);
                    break;

                case PenaltyType.Slowdown:
                    Debug.LogWarning("[Penalty] Slowdown not implemented! Add movement modifier.");
                    break;

                case PenaltyType.Curse:
                    Debug.LogWarning("[Penalty] Curse not implemented! Add BuffType.Curse or similar.");
                    break;

                case PenaltyType.LockAbilities:
                    Debug.LogWarning("[Penalty] LockAbilities not implemented! Hook to your ability system.");
                    break;

                case PenaltyType.None:
                default:
                    break;
            }

            Debug.Log($"[Penalty] Applied: {type}");
        }
    }
}
