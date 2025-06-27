using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Feedbacks;

namespace PalbaGames.UI
{
    [RequireComponent(typeof(Image))]
    public class ChallengeCallShine : MonoBehaviour
    {
        [SerializeField] private float duration = 1.2f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private MMF_Player feedbacksIn;
        [SerializeField] private MMF_Player feedbacksOut;
        [SerializeField] private GameObject feedbacksOutObject;

        private Material material;
        private bool isHiding = false;

        private void OnEnable()
        {
            var img = GetComponent<Image>();
            material = img.material;

            if (material == null || !material.HasProperty("_ShineLocation") || !material.HasProperty("_ShineGlow"))
            {
                Debug.LogWarning("Material missing required shine properties.");
                return;
            }

            material.EnableKeyword("SHINE_ON");
            feedbacksIn?.PlayFeedbacks();
            StartCoroutine(AnimateShine());
        }

        public void ResetHiding()
        {
            isHiding = false;
        }

        public void ScheduleAutoHide(float timeUntilHide)
        {
            float outDuration = GetHideDuration();
            float delay = Mathf.Max(0f, timeUntilHide - outDuration);
            StartCoroutine(PlayOutEffectAfterDelay(delay));
        }

        private IEnumerator PlayOutEffectAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (!isHiding)
            {
                isHiding = true;

                if (feedbacksOutObject != null)
                    feedbacksOutObject.SetActive(true);

                feedbacksOut?.PlayFeedbacks();

                yield return new WaitForSeconds(GetHideDuration());

                if (feedbacksOutObject != null)
                    feedbacksOutObject.SetActive(false);

                gameObject.SetActive(false);
            }
        }

        public IEnumerator HideWithDelay()
        {
            if (!isHiding)
            {
                isHiding = true;

                if (feedbacksOutObject != null)
                    feedbacksOutObject.SetActive(true);

                feedbacksOut?.PlayFeedbacks();

                yield return new WaitForSeconds(GetHideDuration());

                if (feedbacksOutObject != null)
                    feedbacksOutObject.SetActive(false);

                gameObject.SetActive(false);
            }
        }

        public float GetHideDuration()
        {
            return feedbacksOut?.TotalDuration > 0f ? feedbacksOut.TotalDuration : 0.3f;
        }

        private IEnumerator AnimateShine()
        {
            yield return new WaitForSeconds(1f); // počkaj 1 sekundu pred spustením

            float t = 0f;
            while (t < duration)
            {
                float progress = t / duration;
                float eased = curve.Evaluate(progress);
                material.SetFloat("_ShineLocation", eased);

                float glow = Mathf.Clamp01(1f - Mathf.Abs(progress - 0.5f) * 2f);
                material.SetFloat("_ShineGlow", glow);

                t += Time.deltaTime;
                yield return null;
            }

            material.SetFloat("_ShineLocation", 0f);
            material.SetFloat("_ShineGlow", 0f);
        }
    }
}
