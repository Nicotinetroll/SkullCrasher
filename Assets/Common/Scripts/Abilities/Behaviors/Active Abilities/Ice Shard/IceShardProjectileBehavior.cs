using UnityEngine;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Handles logic for flying Ice Shard projectiles in camera space.
    /// </summary>
    public class IceShardProjectileBehavior : CameraSpaceProjectile
    {
        [SerializeField] BoxCollider2D boxCollider;

        public override Rect GetBounds()
        {
            var center = transform.position + transform.rotation * boxCollider.offset;
            return new Rect(center, transform.rotation * boxCollider.size);
        }

        public void SetData(float size, float damageMultiplier, float speed)
        {
            transform.localScale = Vector3.one * size * PlayerBehavior.Player.SizeMultiplier;

            DamageMultiplier = damageMultiplier;
            Speed = speed;

            // 🔥 Priraď AbilityType pre tracking
            SourceAbilityType = AbilityType.IceShard;
        }

        public override void Update()
        {
            base.Update();

            transform.rotation = Quaternion.FromToRotation(Vector3.up, Direction);
        }
    }
}