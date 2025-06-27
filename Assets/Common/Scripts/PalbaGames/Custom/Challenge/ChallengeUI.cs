using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PalbaGames.Challenges
{
    /// <summary>
    /// Simple Challenge UI that shows: Intro → Progress → Result
    /// </summary>
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
            DebugLogger.Log("[ChallengeUI] === AWAKE DEBUG ===");
            
            // Hide all blocks at start
            if (introBlock != null) 
            {
                DebugLogger.Log($"[ChallengeUI] Intro block found: {introBlock.name}, initial state: {introBlock.activeSelf}");
                introBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Intro block after SetActive(false): {introBlock.activeSelf}");
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] Intro block is NULL in Awake!");
            }
            
            if (progressBlock != null) 
            {
                DebugLogger.Log($"[ChallengeUI] Progress block found: {progressBlock.name}, initial state: {progressBlock.activeSelf}");
                progressBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Progress block after SetActive(false): {progressBlock.activeSelf}");
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] Progress block is NULL in Awake!");
            }
            
            if (resultBlock != null) 
            {
                DebugLogger.Log($"[ChallengeUI] Result block found: {resultBlock.name}, initial state: {resultBlock.activeSelf}");
                resultBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Result block after SetActive(false): {resultBlock.activeSelf}");
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] Result block is NULL in Awake!");
            }
            
            DebugLogger.Log("[ChallengeUI] === AWAKE COMPLETE ===");
        }

        private void Update()
        {
            // Update progress display when challenge is running
            if (currentChallenge != null && progressBlock != null && progressBlock.activeInHierarchy)
            {
                UpdateProgressDisplay();
            }
        }

        /// <summary>
        /// Starts the complete challenge UI sequence
        /// </summary>
        public void StartChallengeSequence(BaseChallenge challenge, string description = "")
        {
            DebugLogger.Log($"[ChallengeUI] Starting challenge sequence: {challenge?.GetDisplayName()}");
            
            currentChallenge = challenge;
            
            // Stop any previous sequence
            if (challengeSequence != null)
                StopCoroutine(challengeSequence);
                
            challengeSequence = StartCoroutine(RunChallengeSequence(description));
        }

        private IEnumerator RunChallengeSequence(string description)
        {
            // Phase 1: Show Intro
            yield return StartCoroutine(ShowIntro(description));
            
            // Phase 2: Show Progress (waits until challenge completes)
            yield return StartCoroutine(ShowProgress());
            
            // Phase 3: Show Result
            yield return StartCoroutine(ShowResult());
            
            // Cleanup
            currentChallenge = null;
            HideAllBlocks();
        }

        private IEnumerator ShowIntro(string description)
        {
            DebugLogger.Log("[ChallengeUI] === STARTING ShowIntro ===");
            
            // Hide other blocks first
            if (progressBlock != null) 
            {
                DebugLogger.Log($"[ChallengeUI] Progress block before hide: {progressBlock.activeSelf}");
                progressBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Progress block after hide: {progressBlock.activeSelf}");
            }
            
            if (resultBlock != null) 
            {
                DebugLogger.Log($"[ChallengeUI] Result block before hide: {resultBlock.activeSelf}");
                resultBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Result block after hide: {resultBlock.activeSelf}");
            }
            
            // Then show intro
            if (introBlock != null)
            {
                DebugLogger.Log($"[ChallengeUI] === INTRO BLOCK DEBUG ===");
                DebugLogger.Log($"[ChallengeUI] Intro block BEFORE SetActive(true): {introBlock.activeSelf}");
                DebugLogger.Log($"[ChallengeUI] Intro block activeInHierarchy BEFORE: {introBlock.activeInHierarchy}");
                
                introBlock.SetActive(true);
                
                DebugLogger.Log($"[ChallengeUI] Intro block AFTER SetActive(true): {introBlock.activeSelf}");
                DebugLogger.Log($"[ChallengeUI] Intro block activeInHierarchy AFTER: {introBlock.activeInHierarchy}");
                DebugLogger.Log($"[ChallengeUI] Intro block GameObject name: {introBlock.name}");
                DebugLogger.Log($"[ChallengeUI] Intro block parent active: {introBlock.transform.parent?.gameObject.activeSelf}");
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] INTRO BLOCK IS NULL!");
            }
            
            // Set intro text
            if (introTitle != null)
            {
                DebugLogger.Log($"[ChallengeUI] Setting intro title to: NEW CHALLENGE");
                introTitle.text = "NEW CHALLENGE";
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] Intro title is NULL!");
            }
                
            if (introDescription != null)
            {
                string desc = string.IsNullOrEmpty(description) 
                    ? currentChallenge?.GetDisplayName() ?? "Challenge" 
                    : description;
                DebugLogger.Log($"[ChallengeUI] Setting intro description to: {desc}");
                introDescription.text = desc;
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] Intro description is NULL!");
            }
            
            DebugLogger.Log($"[ChallengeUI] Waiting {introDisplayTime} seconds...");
            
            // Wait for intro duration
            yield return new WaitForSeconds(introDisplayTime);
            
            // Hide intro
            if (introBlock != null)
            {
                DebugLogger.Log($"[ChallengeUI] Intro block BEFORE SetActive(false): {introBlock.activeSelf}");
                introBlock.SetActive(false);
                DebugLogger.Log($"[ChallengeUI] Intro block AFTER SetActive(false): {introBlock.activeSelf}");
            }
            
            DebugLogger.Log("[ChallengeUI] === ShowIntro COMPLETE ===");
        }

        private IEnumerator ShowProgress()
        {
            DebugLogger.Log("[ChallengeUI] Showing progress phase");
            
            // Hide other blocks
            if (introBlock != null) introBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);
            
            // Show progress
            if (progressBlock != null)
            {
                progressBlock.SetActive(true);
                DebugLogger.Log($"[ChallengeUI] Progress block enabled: {progressBlock.activeInHierarchy}");
            }
            
            // First wait until challenge actually starts (exists in ChallengeManager)
            DebugLogger.Log("[ChallengeUI] Waiting for challenge to start...");
            while (ChallengeManager.Instance == null || ChallengeManager.Instance.CurrentChallenge == null)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            DebugLogger.Log("[ChallengeUI] Challenge started, now monitoring progress");
            
            // Now wait until challenge completes
            while (ChallengeManager.Instance != null && ChallengeManager.Instance.CurrentChallenge != null)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            DebugLogger.Log("[ChallengeUI] Challenge completed, hiding progress");
            
            // Hide progress
            if (progressBlock != null)
            {
                progressBlock.SetActive(false);
                DebugLogger.Log("[ChallengeUI] Progress block disabled");
            }
            
            DebugLogger.Log("[ChallengeUI] Progress phase complete");
        }

        private IEnumerator ShowResult()
        {
            DebugLogger.Log("[ChallengeUI] Showing result phase");
            
            // Make sure we have the final challenge state
            bool success = currentChallenge?.WasSuccessful ?? false;
            DebugLogger.Log($"[ChallengeUI] Challenge success state: {success}");
            
            // Hide other blocks
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);
            
            // Show result
            if (resultBlock != null)
            {
                resultBlock.SetActive(true);
                DebugLogger.Log($"[ChallengeUI] Result block enabled: {resultBlock.activeInHierarchy}");
                
                // Debug position
                var rectTransform = resultBlock.GetComponent<RectTransform>();
                DebugLogger.Log($"[ChallengeUI] Result block position: {rectTransform.anchoredPosition}");
            }
            
            if (resultText != null)
            {
                resultText.text = success 
                    ? "<color=#00FF00>✔ CHALLENGE COMPLETE!</color>"
                    : "<color=#FF0000>✘ CHALLENGE FAILED</color>";
                DebugLogger.Log($"[ChallengeUI] Set result text: {resultText.text}");
            }
            else
            {
                DebugLogger.LogError("[ChallengeUI] resultText is NULL!");
            }
            
            // Show result for specified time
            DebugLogger.Log($"[ChallengeUI] Showing result for {resultDisplayTime} seconds");
            yield return new WaitForSeconds(resultDisplayTime);
            
            // Hide result
            if (resultBlock != null)
            {
                resultBlock.SetActive(false);
                DebugLogger.Log("[ChallengeUI] Result block disabled");
            }
            
            DebugLogger.Log("[ChallengeUI] Result phase complete");
        }

        private void UpdateProgressDisplay()
        {
            if (currentChallenge == null) 
            {
                DebugLogger.Log("[ChallengeUI] UpdateProgressDisplay: currentChallenge is NULL");
                return;
            }
            
            // Update progress text
            if (progressText != null)
            {
                string progressString = currentChallenge.GetProgressString();
                progressText.text = progressString;
                DebugLogger.Log($"[ChallengeUI] Progress text updated: {progressString}");
            }
            else
            {
                DebugLogger.LogWarning("[ChallengeUI] progressText is NULL!");
            }
                
            // Update time left
            if (timeLeftText != null)
            {
                string timeString = $"{Mathf.CeilToInt(currentChallenge.RemainingTime)}s";
                timeLeftText.text = timeString;
                DebugLogger.Log($"[ChallengeUI] Time text updated: {timeString}");
            }
            else
            {
                DebugLogger.LogWarning("[ChallengeUI] timeLeftText is NULL!");
            }
                
            // Update progress slider
            if (progressSlider != null)
            {
                // Use time-based progress as fallback
                float timeProgress = 1f - (currentChallenge.RemainingTime / currentChallenge.TimeLimit);
                progressSlider.value = Mathf.Clamp01(timeProgress);
                DebugLogger.Log($"[ChallengeUI] Slider updated: {progressSlider.value}");
            }
        }

        private void HideAllBlocks()
        {
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);
            if (resultBlock != null) resultBlock.SetActive(false);
        }

        /// <summary>
        /// Force stop current sequence and hide UI
        /// </summary>
        public void ForceHide()
        {
            if (challengeSequence != null)
            {
                StopCoroutine(challengeSequence);
                challengeSequence = null;
            }
            
            currentChallenge = null;
            HideAllBlocks();
            DebugLogger.Log("[ChallengeUI] Force hidden");
        }

        /// <summary>
        /// Manual method to show result (for external use)
        /// </summary>
        public void ShowResultManual(bool success, string customText = "")
        {
            // Hide other blocks
            if (introBlock != null) introBlock.SetActive(false);
            if (progressBlock != null) progressBlock.SetActive(false);
            
            // Show result
            if (resultBlock != null)
            {
                resultBlock.SetActive(true);
                DebugLogger.Log($"[ChallengeUI] Manual result block enabled: {resultBlock.activeInHierarchy}");
            }
            
            if (resultText != null)
            {
                if (!string.IsNullOrEmpty(customText))
                {
                    resultText.text = customText;
                }
                else
                {
                    resultText.text = success 
                        ? "<color=#00FF00>✔ CHALLENGE COMPLETE!</color>"
                        : "<color=#FF0000>✘ CHALLENGE FAILED</color>";
                }
                DebugLogger.Log($"[ChallengeUI] Manual result text set: {resultText.text}");
            }
            
            StartCoroutine(HideResultAfterDelay());
        }

        private IEnumerator HideResultAfterDelay()
        {
            yield return new WaitForSeconds(resultDisplayTime);
            resultBlock.SetActive(false);
        }

        /// <summary>
        /// Test method - call from Inspector or debug script
        /// </summary>
        [ContextMenu("Test Intro")]
        public void TestIntro()
        {
            HideAllBlocks();
            introBlock.SetActive(true);
            if (introTitle != null) introTitle.text = "TEST CHALLENGE";
            if (introDescription != null) introDescription.text = "This is a test description";
        }

        [ContextMenu("Test Progress")]
        public void TestProgress()
        {
            HideAllBlocks();
            progressBlock.SetActive(true);
            if (progressText != null) progressText.text = "5/10 KILLS";
            if (timeLeftText != null) timeLeftText.text = "15s";
        }

        [ContextMenu("Test Result Success")]
        public void TestResultSuccess()
        {
            ShowResultManual(true);
        }

        [ContextMenu("Test Result Fail")]
        public void TestResultFail()
        {
            ShowResultManual(false);
        }
    }
}