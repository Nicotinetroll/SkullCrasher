using UnityEngine;
using OctoberStudio;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Simplified reward system with direct drop functionality.
    /// Stat modifications are handled by StatBuffManager and StatPenaltyManager.
    /// </summary>
    public static class RewardSystem
    {
        /// <summary>
        /// Spawn items at specific position with radius distribution.
        /// </summary>
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

        /// <summary>
        /// Spawn items around player position.
        /// </summary>
        public static void ApplyDropAroundPlayer(DropType dropType, int count, float radius = 2f)
        {
            Vector2 playerPos = new Vector2(
                PlayerBehavior.Player.transform.position.x,
                PlayerBehavior.Player.transform.position.y
            );

            ApplyDropAtPosition(dropType, playerPos, count, radius);
        }
    }
}