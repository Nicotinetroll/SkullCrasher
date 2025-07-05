using UnityEngine;
using MoreMountains.Feedbacks;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Feel feedback for triggering challenge progress particles.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("Triggers particle emission for challenge progress")]
    [FeedbackPath("UI/Challenge Particles")]
    public class MMF_ChallengeParticles : MMFeedback
    {
        [Header("Challenge Particles")]
        [Tooltip("ParticleSystem to emit from")]
        public ParticleSystem targetParticleSystem;
        
        [Tooltip("Enable intensity control from Timeline")]
        public bool useIntensityControl = true;
        
        [Tooltip("Base intensity multiplier")]
        [Range(0.1f, 3.0f)]
        public float baseIntensity = 1.0f;
        
        [Header("Positioning")]
        [Tooltip("Should the particle system move to follow slider fill edge?")]
        public bool followSliderFill = true;
        
        [Tooltip("Target slider to follow")]
        public UnityEngine.UI.Slider targetSlider;
        
        [Tooltip("Offset from fill edge position")]
        public Vector3 positionOffset = Vector3.zero;

        // Runtime intensity control
        private static float runtimeIntensity = 1.0f;
        private ParticleSystem cachedParticleSystem;
        private RectTransform fillArea;

        public static void SetGlobalIntensity(float intensity)
        {
            runtimeIntensity = Mathf.Clamp(intensity, 0.1f, 3.0f);
        }

        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);
            
            if (targetParticleSystem == null)
                targetParticleSystem = Owner.GetComponentInChildren<ParticleSystem>();
            
            cachedParticleSystem = targetParticleSystem;
            
            if (targetSlider == null)
                targetSlider = Owner.GetComponentInParent<UnityEngine.UI.Slider>();
            
            if (targetSlider != null)
                fillArea = targetSlider.fillRect;
                
            if (cachedParticleSystem != null)
            {
                var emission = cachedParticleSystem.emission;
                emission.enabled = false;
                emission.rateOverTime = 0f;
            }
        }

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || cachedParticleSystem == null) return;

            if (followSliderFill && targetSlider != null)
            {
                UpdateParticlePosition();
            }

            cachedParticleSystem.Emit(1);
            Debug.Log("[MMF_ChallengeParticles] Triggered particle emission");
        }

        private void UpdateParticlePosition()
        {
            if (targetSlider == null || fillArea == null) return;

            float sliderValue = targetSlider.value;
            Rect fillRect = fillArea.rect;
            float fillWidth = fillRect.width * sliderValue;
            
            Vector3 localEdgePos = new Vector3(fillRect.xMin + fillWidth, fillRect.center.y, 0f);
            Vector3 finalPosition = localEdgePos + positionOffset;

            cachedParticleSystem.transform.localPosition = finalPosition;
        }

        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || cachedParticleSystem == null) return;
            cachedParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}