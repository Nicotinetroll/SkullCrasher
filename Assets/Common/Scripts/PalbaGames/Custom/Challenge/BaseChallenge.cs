namespace PalbaGames.Challenges
{
    public abstract class BaseChallenge
    {
        protected float timeLimit;
        protected float timeRemaining;

        public float TimeLimit => timeLimit;
        public float RemainingTime => timeRemaining;
        public bool WasSuccessful { get; protected set; }

        public abstract void Start();
        public abstract bool UpdateChallenge(float deltaTime);
        public abstract string GetDisplayName();
        public abstract string GetProgressString();

        /// <summary>
        /// Optional visual progress (0 to 1).
        /// </summary>
        public virtual float ProgressNormalized => 0f;
    }
}