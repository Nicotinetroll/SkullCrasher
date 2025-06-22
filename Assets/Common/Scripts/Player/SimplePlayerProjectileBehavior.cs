using OctoberStudio.Easing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio
{
    /// <summary>
    /// Basic player projectile with simple forward movement and impact logic.
    /// </summary>
    public class SimplePlayerProjectileBehavior : ProjectileBehavior
    {
        [SerializeField] float speed;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] TrailRenderer trail;
        [SerializeField] float lifetime = -1;
        [SerializeField] Transform rotatingPart;
        [SerializeField] bool selfDestructOnHit = true;
        [SerializeField] List<ParticleSystem> particles;

        private Vector3 direction;
        private float spawnTime;

        public float Speed { get; set; }
        public float LifeTime { get; set; }

        IEasingCoroutine scaleCoroutine;

        public UnityAction<SimplePlayerProjectileBehavior> onFinished;

        /// <summary>
        /// Initialize projectile with position and direction.
        /// </summary>
        public void Init(Vector2 position, Vector2 direction)
        {
            base.Init();

            transform.position = position;
            transform.localScale = Vector3.one * PlayerBehavior.Player.SizeMultiplier;
            this.direction = direction;

            foreach (var particle in particles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Clear();
                particle.Play();
            }

            if (rotatingPart != null)
                rotatingPart.rotation = Quaternion.FromToRotation(Vector2.up, direction);

            Speed = speed * PlayerBehavior.Player.ProjectileSpeedMultiplier;
            spawnTime = Time.time;
            LifeTime = lifetime * PlayerBehavior.Player.DurationMultiplier;

            trail?.Clear();
        }

        /// <summary>
        /// Initialize projectile with source ability type.
        /// </summary>
        public void Init(Vector2 position, Vector2 direction, AbilityType sourceAbility)
        {
            Init(position, direction);
            SourceAbilityType = sourceAbility;
        }

        private void Update()
        {
            if (spriteRenderer != null && !spriteRenderer.isVisible)
            {
                Clear();
                onFinished?.Invoke(this);
                return;
            }

            transform.position += direction * Time.deltaTime * Speed;

            if (LifeTime > 0 && Time.time - spawnTime > LifeTime)
            {
                Clear();
                onFinished?.Invoke(this);
            }
        }

        public IEasingCoroutine ScaleRotatingPart(Vector3 initialScale, Vector3 targetScale)
        {
            if (rotatingPart == null) return null;

            rotatingPart.localScale = initialScale;
            scaleCoroutine = rotatingPart.DoLocalScale(targetScale, 0.25f);
            return scaleCoroutine;
        }

        public void Clear()
        {
            foreach (var particle in particles)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle.Clear();
            }

            scaleCoroutine.StopIfExists();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (selfDestructOnHit)
            {
                Clear();
                onFinished?.Invoke(this);
            }
        }
    }
}
