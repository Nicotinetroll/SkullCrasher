using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using UnityEngine;
using UnityEngine.Events;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Handles logic for a falling meteor projectile that damages enemies on impact.
    /// </summary>
    public class MeteorProjectileBehavior : MonoBehaviour
    {
        public static readonly int METEOR_LAUNCH_HASH = "Meteor Launch".GetHashCode();
        public static readonly int METEOR_IMPACT_HASH = "Meteor Impact".GetHashCode();

        [SerializeField] ParticleSystem explosionParticle;
        [SerializeField] GameObject visuals;

        [SerializeField] float speed = 4f;

        public event UnityAction<MeteorProjectileBehavior> onFinished;

        private IEasingCoroutine movementCoroutine;
        private IEasingCoroutine disableCoroutine;

        public float DamageMultiplier { get; set; }
        public float ExplosionRadius { get; set; }

        /// <summary>
        /// The ability that spawned this meteor, used for damage tracking.
        /// </summary>
        public AbilityType SourceAbilityType { get; set; } = AbilityType.None;

        public void Init(Vector2 impactPosition)
        {
            var spawnPosition = impactPosition + (visuals.transform.rotation * Vector3.up).XY() * CameraManager.HalfHeight * 2.2f;

            visuals.SetActive(true);

            movementCoroutine.StopIfExists();
            disableCoroutine.StopIfExists();

            transform.position = spawnPosition;

            var distance = Vector2.Distance(impactPosition, spawnPosition);
            var duration = distance / speed;
            movementCoroutine = transform.DoPosition(impactPosition, duration).SetOnFinish(Explode);

            GameController.AudioManager.PlaySound(METEOR_LAUNCH_HASH);
        }

        private void Explode()
        {
            visuals.SetActive(false);

            var enemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, ExplosionRadius);
            float baseDamage = PlayerBehavior.Player.Damage * DamageMultiplier;

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];

                float finalDamage = baseDamage;
                bool isCrit = false;

                if (PlayerBehavior_Extended.Instance != null)
                {
                    finalDamage = PlayerBehavior_Extended.Instance.GetFinalDamage(baseDamage, out isCrit);
                }

                // 🔥 Tu opravujeme volanie na extended script
                var extended = enemy.GetComponent<EnemyBehavior_Extended>();
                extended?.TakeDamageFromAbility(finalDamage, SourceAbilityType, isCrit);
            }

            explosionParticle.Play();

            disableCoroutine = EasingManager.DoAfter(2.5f, () =>
            {
                gameObject.SetActive(false);
                visuals.SetActive(true);
                onFinished?.Invoke(this);
            });

            GameController.AudioManager.PlaySound(METEOR_IMPACT_HASH);
        }


        public void Clear()
        {
            movementCoroutine.StopIfExists();
            disableCoroutine.StopIfExists();

            gameObject.SetActive(false);
            visuals.SetActive(true);
        }
    }
}
