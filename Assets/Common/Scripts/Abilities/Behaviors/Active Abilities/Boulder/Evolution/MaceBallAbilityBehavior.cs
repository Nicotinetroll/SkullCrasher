using UnityEngine;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Handles initialization and logic of the Mace Ball ability which launches a spinning boulder.
    /// </summary>
    public class MaceBallAbilityBehavior : AbilityBehavior<MaceBallAbilityData, MaceBallAbilityLevel>
    {
        public static readonly int MACE_BALL_LAUNCH_HASH = "Mace Ball Launch".GetHashCode();

        [SerializeField] BoulderProjectileBehavior boulderProjectileBehavior;

        public override void Init(AbilityData data, int stageId)
        {
            base.Init(data, stageId);

            GameController.AudioManager.PlaySound(MACE_BALL_LAUNCH_HASH);

            boulderProjectileBehavior.Direction = (Vector2.up + Vector2.right).normalized;
        }

        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);

            boulderProjectileBehavior.SetData(
                AbilityLevel.Size,
                AbilityLevel.DamageMultiplier,
                AbilityLevel.Speed,
                AbilityLevel.AngularSpeed
            );

            boulderProjectileBehavior.SourceAbilityType = AbilityType.MaceBall;
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}