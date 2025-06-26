namespace PalbaGames.Challenges
{
    /// <summary>
    /// Base class for all runtime challenges.
    /// </summary>
    public abstract class BaseChallenge
    {
        protected float timeLimit;
        protected float timeRemaining;

        public abstract void Start();
        public abstract bool UpdateChallenge(float deltaTime);
    }
}