using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Handles visual representation of active challenge lifecycle: intro → progress → result.
    /// </summary>
    public class ChallengeUI : MonoBehaviour
    {
        [Header("Intro")]
        [SerializeField] private GameObject introBlock;
        [SerializeField] private TextMeshProUGUI challengeTitleText;
        [SerializeField] private TextMeshProUGUI challengeDescText;

        [Header("Progress")]
        [SerializeField] private GameObject progressBlock;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressValueText;
        [SerializeField] private TextMeshProUGUI timeLeftText;

        [Header("Result")]
        [SerializeField] private GameObject resultBlock;
        [SerializeField] private TextMeshProUGUI resultText;

        private BaseChallenge activeChallenge;
        private Coroutine introRoutine;

        private void Update()
        {
            activeChallenge = ChallengeManager.Instance?.CurrentChallenge;

            if (activeChallenge == null)
            {
                progressBlock.SetActive(false);
                return;
            }

            if (!progressBlock.activeSelf)
                progressBlock.SetActive(true);

            progressValueText.text = activeChallenge.GetProgressString();
            timeLeftText.text = $"{activeChallenge.RemainingTime:F1} sec left";

            // Progress bar reflects objective completion, not time
            if (progressSlider != null)
            {
                float progress = activeChallenge.ProgressNormalized;
                progressSlider.value = Mathf.Clamp01(progress);
            }
        }

        public void ShowChallenge(BaseChallenge challenge, string description = "")
        {
            activeChallenge = challenge;

            if (introRoutine != null)
                StopCoroutine(introRoutine);
            introRoutine = StartCoroutine(IntroSequence(description));
        }

        private IEnumerator IntroSequence(string desc)
        {
            introBlock.SetActive(true);
            progressBlock.SetActive(false);
            resultBlock.SetActive(false);

            challengeTitleText.text = "NEW CHALLENGE";
            challengeDescText.text = desc;

            yield return new WaitForSeconds(2f);

            introBlock.SetActive(false);
        }

        public void Hide()
        {
            activeChallenge = null;
            introBlock.SetActive(false);
            progressBlock.SetActive(false);
            resultBlock.SetActive(false);
        }

        public void ShowResult(bool success, string rewardSummary = "")
        {
            introBlock.SetActive(false);
            progressBlock.SetActive(false);
            resultBlock.SetActive(true);

            resultText.text = success
                ? $"<color=#00FF00>✔ CHALLENGE COMPLETE</color>\n<color=#FFD700>{rewardSummary}</color>"
                : $"<color=#FF0000>✘ CHALLENGE FAILED</color>";

            StartCoroutine(AutoHideResult(4f));
        }

        private IEnumerator AutoHideResult(float delay)
        {
            yield return new WaitForSeconds(delay);
            resultBlock.SetActive(false);
        }
    }
}
