using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Launches fast-moving silver projectiles that damage enemies on contact.
    /// </summary>
    public class SilverStakesAbilityBehavior : AbilityBehavior<SilverStakesAbilityData, SilverStakesAbilityLevel>
    {
        public static readonly int SILVER_STAKES_LAUNCH_HASH = "Silver Stakes Launch".GetHashCode();

        [SerializeField] GameObject silverShardPrefab;
        public GameObject SilverShardPrefab => silverShardPrefab;

        private PoolComponent<IceShardProjectileBehavior> projectilePool;
        public List<IceShardProjectileBehavior> projectiles = new();

        private void Awake()
        {
            projectilePool = new PoolComponent<IceShardProjectileBehavior>("Quick Silver Projectile", SilverShardPrefab, 6);
        }

        public override void Init(AbilityData data, int stageId)
        {
            base.Init(data, stageId);

            for (int i = 0; i < AbilityLevel.ProjectilesCount; i++)
            {
                var projectile = projectilePool.GetEntity();

                projectile.transform.position = PlayerBehavior.CenterPosition;
                projectile.SetData(AbilityLevel.ProjectileSize, AbilityLevel.Damage, AbilityLevel.ProjectileSpeed);
                projectile.Direction = Random.onUnitSphere.SetZ(0).normalized;

                // 🔧 Opravený property názov pre tracking
                projectile.SourceAbilityType = AbilityType.SilverStakes;

                projectiles.Add(projectile);
            }

            GameController.AudioManager.PlaySound(SILVER_STAKES_LAUNCH_HASH);
        }

        private void Disable()
        {
            foreach (var projectile in projectiles)
            {
                projectile.gameObject.SetActive(false);
            }

            projectiles.Clear();
        }

        public override void Clear()
        {
            Disable();
            base.Clear();
        }
    }
}
