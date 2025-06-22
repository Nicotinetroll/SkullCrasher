using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Active ability that spawns multiple daggers in a circle, each dealing damage on hit.
    /// </summary>
    public class TwinDaggerAbilityBehavior : AbilityBehavior<TwinDaggerAbilityData, TwinDaggerAbilityLevel>
    {
        public static readonly int TWIN_DAGGERS_HASH = "Twin Daggers Launch".GetHashCode();

        [SerializeField] GameObject daggerPrefab;
        public GameObject DaggerPrefab => daggerPrefab;

        private PoolComponent<SimplePlayerProjectileBehavior> projectilePool;
        private Coroutine abilityCoroutine;
        private readonly List<SimplePlayerProjectileBehavior> projectiles = new();

        private void Awake()
        {
            projectilePool = new PoolComponent<SimplePlayerProjectileBehavior>("Twin Dagger Ability Projectile", DaggerPrefab, 5);
        }

        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);
            Disable();
            abilityCoroutine = StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < AbilityLevel.ProjectileCount; i++)
                {
                    var projectile = projectilePool.GetEntity();

                    var angle = 360f / AbilityLevel.ProjectileCount * i;
                    var direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

                    projectile.Init(PlayerBehavior.CenterPosition, direction, AbilityType.TwinDagger);
                    projectile.DamageMultiplier = AbilityLevel.Damage;
                    projectile.KickBack = true;

                    projectile.onFinished += OnProjectileFinished;
                    projectiles.Add(projectile);
                }

                GameController.AudioManager.PlaySound(TWIN_DAGGERS_HASH);

                yield return new WaitForSeconds(AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier);
            }
        }

        private void Disable()
        {
            if (abilityCoroutine != null)
                StopCoroutine(abilityCoroutine);

            foreach (var projectile in projectiles)
            {
                projectile.onFinished -= OnProjectileFinished;
                projectile.Clear();
            }

            projectiles.Clear();
        }

        private void OnProjectileFinished(SimplePlayerProjectileBehavior projectile)
        {
            projectile.onFinished -= OnProjectileFinished;
            projectiles.Remove(projectile);
        }

        public override void Clear()
        {
            Disable();
            base.Clear();
        }
    }
}
