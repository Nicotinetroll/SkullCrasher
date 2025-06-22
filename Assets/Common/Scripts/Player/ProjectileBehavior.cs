using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio
{
    /// <summary>
    /// Base class for all projectile logic.
    /// </summary>
    public class ProjectileBehavior : MonoBehaviour
    {
        /// <summary>
        /// The ability that spawned this projectile. Used for damage tracking.
        /// </summary>
        public AbilityType SourceAbilityType { get; set; } = AbilityType.None;

        /// <summary>
        /// Optional effects applied on hit.
        /// </summary>
        public List<Effect> Effects { get; private set; }

        /// <summary>
        /// Damage multiplier applied on top of base damage.
        /// </summary>
        public float DamageMultiplier { get; set; }

        /// <summary>
        /// Should the projectile apply knockback on hit.
        /// </summary>
        public bool KickBack { get; set; }

        public virtual void Init()
        {
            Effects = new List<Effect>();
        }
    }
}