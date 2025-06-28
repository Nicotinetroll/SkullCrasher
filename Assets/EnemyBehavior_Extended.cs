using UnityEngine;
using OctoberStudio;
using OctoberStudio.Enemy;
using FunkyCode; // Smart Lighting 2D

namespace PalbaGames
{
    /// <summary>
    /// Extended enemy behavior that handles ability-type damage tracking and lighting effects.
    /// </summary>
    [RequireComponent(typeof(EnemyBehavior))]
    public class EnemyBehavior_Extended : MonoBehaviour
    {
        [Header("Lighting Effects")]
        [SerializeField] private bool enableDeathLightingEffect = true;
        [SerializeField] private LightCollider2D lightCollider;
        [SerializeField] private float defaultShadowTranslucency = 0.652f;
        [SerializeField] private bool disableOriginalShadowAnimation = true; // 🔥 Nový flag
        
        private EnemyBehavior _enemy;

        private void Awake()
        {
            _enemy = GetComponent<EnemyBehavior>();
            
            // Auto-find light collider if not assigned
            if (lightCollider == null)
            {
                lightCollider = GetComponent<LightCollider2D>();
                
                if (lightCollider == null)
                {
                    lightCollider = GetComponentInChildren<LightCollider2D>();
                }
            }
            
            if (lightCollider == null)
            {
                //Debug.LogWarning($"LightCollider2D not found on {gameObject.name} or its children!");
            }
        }

        private void OnEnable()
        {
            ResetShadowToDefault();
        }

        private void Start()
        {
            _enemy.onEnemyDied += HandleEnemyDeath;
            
            // 🔥 Override Die method if we want to disable original shadow animation
            if (disableOriginalShadowAnimation)
            {
                // Hack: Replace original Die method behavior
                _enemy.onEnemyDied += DisableOriginalShadowAnimation;
            }
            
            ResetShadowToDefault();
        }

        /// <summary>
        /// Disable the original shadowSprite.DoAlpha animation
        /// </summary>
        private void DisableOriginalShadowAnimation(EnemyBehavior enemy)
        {
            // 🔥 Find shadowSprite and stop any animation on it
            var shadowSprites = enemy.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in shadowSprites)
            {
                if (sprite.name.ToLower().Contains("shadow"))
                {
                 //   Debug.Log($"🔥 Found shadow sprite: {sprite.name} - stopping animations");
                    // Stop any ongoing animations on shadow sprite
                    // sprite.DOKill(); // if using DoTween
                }
            }
        }

        public void ResetShadowToDefault()
        {
            if (lightCollider != null)
            {
                lightCollider.shadowTranslucency = defaultShadowTranslucency;
             //   Debug.Log($"🔥 RESET: Set shadow translucency to {defaultShadowTranslucency} for {gameObject.name}");
            }
        }

        private void OnDestroy()
        {
            if (_enemy != null)
            {
                _enemy.onEnemyDied -= HandleEnemyDeath;
                if (disableOriginalShadowAnimation)
                    _enemy.onEnemyDied -= DisableOriginalShadowAnimation;
            }
        }

        /// <summary>
        /// Handle enemy death and trigger lighting effects with delayed execution
        /// </summary>
        private void HandleEnemyDeath(EnemyBehavior enemy)
        {
        //    Debug.Log($"🔥 DEATH: Enemy died: {enemy.name}");
            
            if (enableDeathLightingEffect && lightCollider != null && GlobalLightingController.Instance != null)
            {
                // 🔥 Čakaj 1 frame aby sa spustili originál animácie, potom ich override
                StartCoroutine(DelayedShadowFade());
            }
        }

        /// <summary>
        /// Delayed shadow fade to override original animations
        /// </summary>
        private System.Collections.IEnumerator DelayedShadowFade()
        {
            yield return null; // Čakaj 1 frame
            
            float currentValue = lightCollider.shadowTranslucency;
            //Debug.Log($"🔥 DELAYED: Starting fade from {currentValue} to 1");
            
            GlobalLightingController.Instance.FadeEnemyShadow(lightCollider, currentValue);
        }

        public void TakeDamageFromAbility(float amount, AbilityType source, bool isCritical = false)
        {
            if (!_enemy.IsAlive || _enemy.IsInvulnerable) return;

            DamageTracker.Instance?.ReportDamage(source, amount);
            _enemy.TakeDamage(amount, isCritical);
        }
    }
}