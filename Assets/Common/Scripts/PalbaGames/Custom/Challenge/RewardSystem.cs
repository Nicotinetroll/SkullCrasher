using UnityEngine;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    public static class RewardSystem
    {
        public static void Apply(RewardType type)
        {
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
                case RewardType.None:
                default:
                    break;
            }

            Debug.Log($"[Reward] Applied: {type}");
        }

        public static void ApplyDrop(DropType dropType, int count)
        {
            var pos = new Vector2(
                PlayerBehavior.Player.transform.position.x,
                PlayerBehavior.Player.transform.position.y
            );

            for (int i = 0; i < count; i++)
            {
                StageController.DropManager.Drop(dropType, pos);
            }

            Debug.Log($"[Reward] Dropped {count}x {dropType}");
        }

        public static void ApplyPenalty(PenaltyType type)
        {
            switch (type)
            {
                case PenaltyType.TakeDamage:
                    PlayerBehavior.Player.TakeDamage(50);
                    break;
                case PenaltyType.Slowdown:
                    Debug.LogWarning("[Penalty] Slowdown not implemented");
                    break;
                case PenaltyType.Curse:
                    Debug.LogWarning("[Penalty] Curse not implemented");
                    break;
                case PenaltyType.LockAbilities:
                    Debug.LogWarning("[Penalty] LockAbilities not implemented");
                    break;
                case PenaltyType.None:
                default:
                    break;
            }

            Debug.Log($"[Penalty] Applied: {type}");
        }
    }
}
