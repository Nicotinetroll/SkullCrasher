using UnityEngine;
using FunkyCode;

namespace PalbaGames
{
    /// <summary>
    /// Global controller for managing Smart Lighting 2D effects throughout the game
    /// </summary>
    public class GlobalLightingController : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float shadowFadeOutDuration = 1f;
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private static GlobalLightingController instance;
        public static GlobalLightingController Instance 
        { 
            get 
            { 
                if (instance == null)
                    instance = FindObjectOfType<GlobalLightingController>();
                return instance; 
            } 
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Fade out enemy shadow when it dies (from start value to 0 = full shadow/invisible)
        /// </summary>
        public void FadeEnemyShadow(LightCollider2D lightCollider, float startValue)
        {
            if (lightCollider != null)
            {
                StartCoroutine(FadeShadowCoroutine(lightCollider, startValue));
            }
        }

        /// <summary>
        /// Fade out enemy shadow when it dies (overload with automatic start value detection)
        /// </summary>
        public void FadeEnemyShadow(LightCollider2D lightCollider)
        {
            FadeEnemyShadow(lightCollider, lightCollider.shadowTranslucency);
        }

        private System.Collections.IEnumerator FadeShadowCoroutine(LightCollider2D lightCollider, float startTranslucency)
        {
            float elapsedTime = 0f;

            // Debug.Log($"🔥 FADE START: From {startTranslucency} to 1 over {shadowFadeOutDuration}s");

            // 🔥 Force set starting value to override any conflicting animations
            lightCollider.shadowTranslucency = startTranslucency;

            while (elapsedTime < shadowFadeOutDuration && lightCollider != null)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / shadowFadeOutDuration;
                float curveValue = fadeOutCurve.Evaluate(progress);
                
                // 🔥 Interpoluj od štartovej hodnoty k 1 (žiadny tieň)
                float newValue = Mathf.Lerp(startTranslucency, 1f, curveValue);
                lightCollider.shadowTranslucency = newValue;
                
                // Debug každých 0.3s
                if (elapsedTime % 0.3f < Time.deltaTime)
                {
                 //   Debug.Log($"🔥 FADE: Progress {progress:F2}, Value {newValue:F3}");
                }
                
                yield return null;
            }

            // Zaisti že skončí presne na 1 (žiadny tieň)
            if (lightCollider != null)
            {
                lightCollider.shadowTranslucency = 1f;
               // Debug.Log($"🔥 FADE END: Final value = {lightCollider.shadowTranslucency}");
            }
        }
    }
}