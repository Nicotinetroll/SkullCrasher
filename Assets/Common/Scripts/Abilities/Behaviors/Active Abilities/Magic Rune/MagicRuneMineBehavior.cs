using OctoberStudio.Easing;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Behavior for individual Magic Rune mines that explode on enemy contact or after lifetime.
    /// </summary>
    public class MagicRuneMineBehavior : MonoBehaviour
    {
        private static readonly int MAGIC_RUNE_EXPLOSION_HASH = "Magic Rune Explosion".GetHashCode();

        [SerializeField] CircleCollider2D mineTriggerCollider;
        [SerializeField] GameObject mineVisuals;
        [SerializeField] ParticleSystem explosionParticle;

        public float DamageMultiplier { get; private set; }
        public float DamageRadius { get; private set; }

        /// <summary>
        /// Used for tracking the origin of damage.
        /// </summary>
        public AbilityType SourceAbilityType { get; set; } = AbilityType.None;

        private IEasingCoroutine lifetimeCoroutine;

        public void SetData(MagicRuneAbilityLevel stage)
        {
            var size = stage.MineSize * PlayerBehavior.Player.SizeMultiplier;
            transform.localScale = Vector3.one * size;
            mineTriggerCollider.radius = stage.MineTriggerRadius / size;

            DamageMultiplier = stage.Damage;
            DamageRadius = stage.MineDamageRadius * PlayerBehavior.Player.SizeMultiplier;

            mineVisuals.SetActive(true);

            EasingManager.DoAfter(0.2f, () => mineTriggerCollider.enabled = true);

            lifetimeCoroutine = EasingManager.DoAfter(stage.MineLifetime, Explode);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var enemy = collision.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                Explode();
            }
        }

        private void Explode()
        {
            mineTriggerCollider.enabled = false;
            mineVisuals.SetActive(false);

            var enemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, DamageRadius);
            foreach (var enemy in enemies)
            {
                float baseDamage = PlayerBehavior.Player.Damage;
                float finalDamage = baseDamage * DamageMultiplier;

                bool isCrit = false;
                if (PlayerBehavior_Extended.Instance != null)
                {
                    finalDamage = PlayerBehavior_Extended.Instance.GetFinalDamage(baseDamage * DamageMultiplier, out isCrit);
                }

                var extended = enemy.GetComponent<EnemyBehavior_Extended>();
                extended?.TakeDamageFromAbility(finalDamage, SourceAbilityType, isCrit);
            }

            explosionParticle.Play();

            EasingManager.DoAfter(1f, () => gameObject.SetActive(false));

            lifetimeCoroutine.StopIfExists();

            GameController.AudioManager.PlaySound(MAGIC_RUNE_EXPLOSION_HASH);
        }
    }
}
