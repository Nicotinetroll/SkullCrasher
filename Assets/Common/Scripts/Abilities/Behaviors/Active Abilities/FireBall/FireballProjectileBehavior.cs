using OctoberStudio.Easing;
using PalbaGames;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Handles logic for Fireball projectile with AoE explosion and damage tracking.
    /// </summary>
    public class FireballProjectileBehavior : ProjectileBehavior
    {
        private static readonly int FIREBALL_LAUNCH_HASH = "Fireball Launch".GetHashCode();
        private static readonly int FIREBALL_EXPLOSION_HASH = "Fireball Explosion".GetHashCode();

        [SerializeField] Collider2D fireballCollider;
        [SerializeField] ParticleSystem explosionParticle;
        [SerializeField] GameObject visuals;

        private IEasingCoroutine movementCoroutine;
        private IEasingCoroutine disableCoroutine;

        public float ExplosionRadius { get; set; }
        public float Lifetime { get; set; }
        public float Speed { get; set; }
        public float Size { get; set; }

        /// <summary>
        /// Marks if the projectile is a critical hit.
        /// </summary>
        public bool IsCritical { get; set; }

        public event UnityAction<FireballProjectileBehavior> onFinished;

        public void Init()
        {
            base.Init();

            SourceAbilityType = AbilityType.Fireball;

            transform.localScale = Vector3.one * Size * PlayerBehavior.Player.SizeMultiplier;

            var distance = Speed * Lifetime * PlayerBehavior.Player.DurationMultiplier;
            var selfDestructPosition = transform.position + transform.rotation * Vector3.up * distance;

            movementCoroutine = transform.DoPosition(selfDestructPosition, Lifetime / PlayerBehavior.Player.ProjectileSpeedMultiplier).SetOnFinish(Explode);

            visuals.SetActive(true);
            fireballCollider.enabled = true;

            GameController.AudioManager.PlaySound(FIREBALL_LAUNCH_HASH);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<EnemyBehavior>();

            if (enemy != null)
            {
                movementCoroutine.Stop();
                Explode();
            }
        }

        private void Explode()
        {
            fireballCollider.enabled = false;
            visuals.SetActive(false);

            var enemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, ExplosionRadius);

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                var extended = enemy.GetComponent<EnemyBehavior_Extended>();

                extended?.TakeDamageFromAbility(PlayerBehavior.Player.Damage * DamageMultiplier, SourceAbilityType, IsCritical);
            }

            explosionParticle.Play();

            disableCoroutine = EasingManager.DoAfter(1f, () =>
            {
                gameObject.SetActive(false);
                onFinished?.Invoke(this);
            });

            GameController.AudioManager.PlaySound(FIREBALL_EXPLOSION_HASH);
        }

        public void Clear()
        {
            movementCoroutine.StopIfExists();
            disableCoroutine.StopIfExists();

            gameObject.SetActive(false);

            visuals.SetActive(true);
            fireballCollider.enabled = true;

            IsCritical = false;
        }
    }
}
