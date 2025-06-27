namespace PalbaGames.Challenges
{
    /// <summary>
    /// Base class for all challenge types with enhanced progress tracking.
    /// </summary>
    public abstract class BaseChallenge
    {
        protected float timeLimit;
        protected float timeRemaining;

        /// <summary>Set by manager when challenge finishes.</summary>
        public bool WasSuccessful { get; protected set; }

        /// <summary>Time limit for this challenge.</summary>
        public float TimeLimit => timeLimit;

        /// <summary>Time remaining in seconds.</summary>
        public virtual float RemainingTime => timeRemaining;

        /// <summary>
        /// Progress as normalized value (0.0 to 1.0) based on objective completion.
        /// Override in derived classes for objective-specific progress.
        /// </summary>
        public virtual float ProgressNormalized => 1f - (timeRemaining / timeLimit);

        /// <summary>Start the challenge logic.</summary>
        public abstract void Start();

        /// <summary>
        /// Update challenge logic. Returns true when challenge is complete.
        /// </summary>
        public abstract bool UpdateChallenge(float deltaTime);

        /// <summary>Display name for UI.</summary>
        public virtual string GetDisplayName() => GetType().Name;

        /// <summary>Progress string for UI display.</summary>
        public virtual string GetProgressString() => "";

        /// <summary>
        /// Force set the success state (for debugging/testing).
        /// </summary>
        public virtual void SetSuccessState(bool success)
        {
            WasSuccessful = success;
        }
    }
}