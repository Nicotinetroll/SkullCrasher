using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using System.Collections;
using PalbaGames;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio
{
    /// <summary>
    /// Sun ray projectile that locks onto enemies and deals periodic damage.
    /// </summary>
    public class SunRayProjectileBehavior : MonoBehaviour
    {
        public UnityAction<SunRayProjectileBehavior> onFinished;

        public float Lifetime { get; set; }
        public float DamagePerSecond { get; set; }
        public float DamageInterval { get; set; }
        public float AdditionalDamagePerSecond { get; set; }
        public float AdditionalDamageRadius { get; set; }
        public float Speed { get; set; }

        public EnemyBehavior Target { get; private set; }
        public bool IsTargetLocked { get; private set; }

        private IEasingCoroutine movementCoroutine;
        private Coroutine waitingCoroutine;

        private float spawnTime;
        private float lastDamageTime;
        private Vector3 offset;

        // 🔥 Custom ability type for tracking
        public AbilityType SourceAbilityType { get; private set; } = AbilityType.SolarMagnifier;

        public void Spawn(EnemyBehavior target)
        {
            Target = target;

            if (Target != null)
            {
                transform.position = Target.transform.position;
                IsTargetLocked = true;
                Target.onEnemyDied += OnEnemyDied;
            }
            else
            {
                IsTargetLocked = false;
                transform.position = PlayerBehavior.Player.transform.position + new Vector3(-1, 1, 0);
                waitingCoroutine = StartCoroutine(WaitingForTarget());
            }

            spawnTime = Time.time;
            lastDamageTime = Time.time - 100000;
            offset = Random.onUnitSphere.XY().normalized * 0.075f;
        }

        private IEnumerator WaitingForTarget()
        {
            while (Target == null)
            {
                yield return null;
                Target = StageController.EnemiesSpawner.GetClosestEnemy(transform.position);
            }

            MoveToTarget();
        }

        private void Update()
        {
            if (IsTargetLocked && Target != null)
            {
                transform.position = Target.transform.position + offset;
            }

            if (Time.time > lastDamageTime + DamageInterval)
            {
                float totalDamage = DamagePerSecond * PlayerBehavior.Player.Damage * DamageInterval;

                if (IsTargetLocked && Target != null)
                {
                    var extended = Target.GetComponent<EnemyBehavior_Extended>();
                    extended?.TakeDamageFromAbility(totalDamage, SourceAbilityType);
                }

                var closeEnemies = StageController.EnemiesSpawner.GetEnemiesInRadius(transform.position, AdditionalDamageRadius);
                foreach (var enemy in closeEnemies)
                {
                    if (enemy != Target)
                    {
                        float splash = AdditionalDamagePerSecond * PlayerBehavior.Player.Damage * DamageInterval;
                        var extended = enemy.GetComponent<EnemyBehavior_Extended>();
                        extended?.TakeDamageFromAbility(splash, SourceAbilityType);
                    }
                }

                lastDamageTime = Time.time;
            }

            if (Time.time > spawnTime + Lifetime)
            {
                onFinished?.Invoke(this);
                Disable();
            }
        }

        private void MoveToTarget()
        {
            Target.onEnemyDied += OnEnemyDied;

            float dist = Vector2.Distance(transform.position, Target.transform.position);
            float time = dist / (Speed * PlayerBehavior.Player.ProjectileSpeedMultiplier);

            movementCoroutine.StopIfExists();
            movementCoroutine = transform.DoPosition(Target.transform, time)
                .SetEasing(EasingType.SineIn)
                .SetOnFinish(() => IsTargetLocked = true);
        }

        private void OnEnemyDied(EnemyBehavior enemy)
        {
            Target.onEnemyDied -= OnEnemyDied;
            IsTargetLocked = false;

            Target = StageController.EnemiesSpawner.GetClosestEnemy(transform.position);

            if (Target != null)
            {
                MoveToTarget();
            }
            else
            {
                waitingCoroutine = StartCoroutine(WaitingForTarget());
            }
        }

        public void Disable()
        {
            if (Target != null)
            {
                Target.onEnemyDied -= OnEnemyDied;
            }

            movementCoroutine.StopIfExists();
            IsTargetLocked = false;

            if (waitingCoroutine != null)
                StopCoroutine(waitingCoroutine);

            gameObject.SetActive(false);
        }
    }
}
