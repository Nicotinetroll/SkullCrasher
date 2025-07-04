using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Periodically launches projectiles towards the nearest enemy.
    /// </summary>
    public class ScepterWeaponAvilityBehavior : AbilityBehavior<ScepterWeaponAbilityData, ScepterWeaponAbilityLevel>
    {
        public static readonly int SEPTER_PROJECTILE_LAUNCH_HASH = "Septer Projectile Launch".GetHashCode();

        [SerializeField] GameObject projectilePrefab;
        public GameObject ProjectilePrefab => projectilePrefab;

        private PoolComponent<SimplePlayerProjectileBehavior> projectilePool;
        public List<SimplePlayerProjectileBehavior> projectiles = new List<SimplePlayerProjectileBehavior>();

        private IEasingCoroutine projectileCoroutine;
        private Coroutine abilityCoroutine;

        private float AbilityCooldown => AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier;

        private void Awake()
        {
            projectilePool = new PoolComponent<SimplePlayerProjectileBehavior>("Scepter Projectile", ProjectilePrefab, 50);
        }

        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);

            if (abilityCoroutine != null) Disable();

            abilityCoroutine = StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            float lastTimeSpawned = Time.time - AbilityCooldown;

            while (true)
            {
                while (lastTimeSpawned + AbilityCooldown < Time.time)
                {
                    float spawnTime = lastTimeSpawned + AbilityCooldown;

                    var projectile = projectilePool.GetEntity();

                    var closestEnemy = StageController.EnemiesSpawner.GetClosestEnemy(PlayerBehavior.CenterPosition);

                    Vector2 direction = Vector2.up;
                    if (closestEnemy != null)
                    {
                        direction = (closestEnemy.Center - PlayerBehavior.CenterPosition).normalized;
                    }

                    float aliveDuration = Time.time - spawnTime;
                    Vector2 position = PlayerBehavior.CenterPosition + direction * aliveDuration * AbilityLevel.ProjectileSpeed * PlayerBehavior.Player.ProjectileSpeedMultiplier;

                    projectile.Init(position, direction);
                    projectile.Speed = AbilityLevel.ProjectileSpeed * PlayerBehavior.Player.ProjectileSpeedMultiplier;
                    projectile.transform.localScale = Vector3.one * AbilityLevel.ProjectileSize * PlayerBehavior.Player.SizeMultiplier;
                    projectile.LifeTime = AbilityLevel.ProjectileLifetime;
                    projectile.DamageMultiplier = AbilityLevel.Damage;

                    // ⬇ Ability type for damage tracking
                    projectile.SourceAbilityType = AbilityType.AncientScepter;

                    projectile.onFinished += OnProjectileFinished;
                    projectiles.Add(projectile);

                    lastTimeSpawned += AbilityCooldown;

                    GameController.AudioManager.PlaySound(SEPTER_PROJECTILE_LAUNCH_HASH);
                }

                yield return null;
            }
        }

        private void OnProjectileFinished(SimplePlayerProjectileBehavior projectile)
        {
            projectile.onFinished -= OnProjectileFinished;
            projectiles.Remove(projectile);
        }

        private void Disable()
        {
            projectileCoroutine.StopIfExists();

            foreach (var projectile in projectiles)
            {
                projectile.gameObject.SetActive(false);
            }

            projectiles.Clear();

            if (abilityCoroutine != null)
                StopCoroutine(abilityCoroutine);
        }

        public override void Clear()
        {
            Disable();
            base.Clear();
        }
    }
}
