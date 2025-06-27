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
        }

        public static void ApplyDrop(DropType dropType, int count)
        {
            Vector2 playerPos = new Vector2(
                PlayerBehavior.Player.transform.position.x,
                PlayerBehavior.Player.transform.position.y
            );

            float dropRadius = 2f;

            for (int i = 0; i < count; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dropRadius;
                Vector2 dropPos = playerPos + offset;

                StageController.DropManager.Drop(dropType, dropPos);
            }
        }

        public static void ApplyDropAtPosition(DropType dropType, Vector2 position, int count, float radius = 2f)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Vector2 dropPos = position + offset;

                StageController.DropManager.Drop(dropType, dropPos);
            }
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
        }
    }
}
