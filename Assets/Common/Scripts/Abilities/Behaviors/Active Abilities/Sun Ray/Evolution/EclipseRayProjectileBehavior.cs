using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Spiral projectile that shrinks toward the player over time.
    /// </summary>
    public class EclipseRayProjectileBehavior : ProjectileBehavior
    {
        public UnityAction<EclipseRayProjectileBehavior> onFinished;

        private Coroutine movementCoroutine;

        public float InitialRadius { get; set; }
        public float ProjectileLifetime { get; set; }
        public float AngularSpeed { get; set; }

        public void Spawn(float startingAngle)
        {
            Init();
            transform.localScale = Vector3.one * PlayerBehavior.Player.SizeMultiplier;

            KickBack = false;

            // ✅ Neprepisuj property, iba nastav
            SourceAbilityType = AbilityType.LunarProjector;

            movementCoroutine = StartCoroutine(MovementCoroutine(startingAngle));
        }

        private IEnumerator MovementCoroutine(float startingAngle)
        {
            float time = 0f;
            float duration = ProjectileLifetime * PlayerBehavior.Player.DurationMultiplier;
            float distance = InitialRadius * PlayerBehavior.Player.SizeMultiplier;
            float currentDistance = distance;

            Vector3 startPosition = PlayerBehavior.Player.transform.position;

            while (time < duration)
            {
                time += Time.deltaTime;

                float angle = startingAngle + time * AngularSpeed * PlayerBehavior.Player.ProjectileSpeedMultiplier;
                currentDistance -= Time.deltaTime * (distance / duration);

                Vector3 position = startPosition + Quaternion.Euler(0, 0, angle) * Vector3.up * currentDistance;
                transform.position = position;

                yield return null;
            }

            gameObject.SetActive(false);
            onFinished?.Invoke(this);
            movementCoroutine = null;
        }

        public void Disable()
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            gameObject.SetActive(false);
        }
    }
}
