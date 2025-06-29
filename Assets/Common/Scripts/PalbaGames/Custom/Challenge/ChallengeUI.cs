using System.Collections;
using PalbaGames.UI;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PalbaGames.Challenges
{
    public class ChallengeUI : MonoBehaviour
    {
        [Header("UI Blocks")]
        [SerializeField] private GameObject introBlock;
        [SerializeField] private GameObject progressBlock;
        [SerializeField] private GameObject resultBlock;

        [Header("Intro Elements")]
        [SerializeField] private TextMeshProUGUI introTitle;
        [SerializeField] private TextMeshProUGUI introDescription;

        [Header("Progress Elements")]
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI timeLeftText;
        [SerializeField] private Slider progressSlider;

        [Header("Result Elements")]
        [SerializeField] private TextMeshProUGUI resultText;

        [Header("Timing")]
        [SerializeField] private float introDisplayTime = 4f;
        [SerializeField] private float resultDisplayTime = 3f;

        private BaseChallenge currentChallenge;
        private Coroutine challengeSequence;

        private void Awake()
        {
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);
        }

        private void Update()
        {
            if (currentChallenge != null && progressBlock != null && progressBlock.activeInHierarchy)
            {
                UpdateProgressDisplay();
            }
        }

        public void StartChallengeSequence(BaseChallenge challenge, string description = "")
        {
            currentChallenge = challenge;

            if (challengeSequence != null)
                StopCoroutine(challengeSequence);

            challengeSequence = StartCoroutine(RunChallengeSequence(description));
        }

        private IEnumerator RunChallengeSequence(string description)
        {
            yield return StartCoroutine(ShowIntro(description));
            yield return StartCoroutine(ShowProgress());
            yield return StartCoroutine(ShowResult());

            currentChallenge = null;
            HideAllBlocks();
        }

        private IEnumerator ShowIntro(string description)
        {
            if (progressBlock != null) progressBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);

            if (introBlock != null)
            {
                Debug.Log("[ChallengeUI] Activating Intro Block");
                
                // Force deactivate and wait a frame before activating to reset OnEnable and state
                introBlock.SetActive(false);
                yield return null;
                introBlock.SetActive(true);

                var shineScheduled = introBlock.GetComponentInChildren<ChallengeCallShine>();
                if (shineScheduled != null)
                {
                    // Reset private isHiding flag via reflection
                    var fi = typeof(ChallengeCallShine).GetField("isHiding", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (fi != null)
                        fi.SetValue(shineScheduled, false);

                    shineScheduled.ScheduleAutoHide(introDisplayTime);
                }

                if (introTitle != null) introTitle.text = "NEW CHALLENGE";

                if (introDescription != null)
                {
                    introDescription.text = string.IsNullOrEmpty(description)
                        ? currentChallenge?.GetDisplayName() ?? "Challenge"
                        : description;
                }
            }

            yield return new WaitForSeconds(introDisplayTime);

            if (introBlock != null)
            {
                var shine = introBlock.GetComponentInChildren<ChallengeCallShine>();
                if (shine != null)
                {
                    yield return shine.HideWithDelay();
                }
                else
                {
                    introBlock.SetActive(false);
                }
            }
        }

        private IEnumerator ShowProgress()
        {
            if (introBlock != null) introBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);

            if (progressBlock != null)
            {
                progressBlock.SetActive(true);

                // Play shake feedback at start
                var feedbacks = progressBlock.GetComponentInChildren<MoreMountains.Feedbacks.MMF_Player>();
                if (feedbacks != null)
                {
                    feedbacks.PlayFeedbacks();
                }
            }

            while (ChallengeManager.Instance == null || ChallengeManager.Instance.CurrentChallenge == null)
                yield return new WaitForSeconds(0.1f);

            while (ChallengeManager.Instance != null && ChallengeManager.Instance.CurrentChallenge != null)
                yield return new WaitForSeconds(0.1f);

            if (progressBlock != null)
                progressBlock.SetActive(false);
        }

        private IEnumerator ShowResult()
        {
            bool success = currentChallenge?.WasSuccessful ?? false;

            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);

            if (resultBlock != null)
            {
                resultBlock.SetActive(true);
            }

            if (resultText != null)
            {
                resultText.text = success
                    ? "<color=#00FF00>CHALLENGE COMPLETE!</color>"
                    : "<color=#FF0000>CHALLENGE FAILED — now you really got fucked up!</color>";
            }

            yield return new WaitForSeconds(resultDisplayTime);

            if (resultBlock != null)
                resultBlock.SetActive(false);
        }

        private float previousSliderValue = -1f;

        private void UpdateProgressDisplay()
        {
            if (currentChallenge == null) return;

            if (progressText != null)
                progressText.text = currentChallenge.GetProgressString();

            if (timeLeftText != null)
                timeLeftText.text = $"{Mathf.CeilToInt(currentChallenge.RemainingTime)}s";

            if (progressSlider != null)
            {
                float objectiveProgress = currentChallenge.ProgressNormalized;
                if (objectiveProgress <= 0f || objectiveProgress > 1f)
                    objectiveProgress = 1f - (currentChallenge.RemainingTime / currentChallenge.TimeLimit);

                if (Mathf.Abs(previousSliderValue - objectiveProgress) > 0.001f)
                {
                    progressSlider.value = Mathf.Clamp01(objectiveProgress);
                    previousSliderValue = progressSlider.value;

                    var feedbacks = progressSlider.GetComponentInParent<MoreMountains.Feedbacks.MMF_Player>();
                    if (feedbacks != null)
                    {
                        feedbacks.PlayFeedbacks();
                    }
                }
            }
        }

        private void HideAllBlocks()
        {
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);
        }

        public void ForceHide()
        {
            if (challengeSequence != null)
            {
                StopCoroutine(challengeSequence);
                challengeSequence = null;
            }

            currentChallenge = null;
            HideAllBlocks();
        }

        public void ShowResultManual(bool success, string customText = "")
        {
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);

            if (resultBlock != null)
                resultBlock.SetActive(true);

            if (resultText != null)
            {
                resultText.text = !string.IsNullOrEmpty(customText)
                    ? customText
                    : (success ? "<color=#00FF00>✔ CHALLENGE COMPLETE!</color>" : "<color=#FF0000>✘ CHALLENGE FAILED</color>");
            }

            StartCoroutine(HideResultAfterDelay());
        }

        private IEnumerator HideResultAfterDelay()
        {
            yield return new WaitForSeconds(resultDisplayTime);
            if (resultBlock != null) resultBlock.SetActive(false);
        }
    }
}
