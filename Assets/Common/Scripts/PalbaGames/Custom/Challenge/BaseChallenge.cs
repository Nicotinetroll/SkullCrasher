public abstract class BaseChallenge
{
    protected float timeLimit;
    protected float timeRemaining;

    /// <summary>Set by manager when challenge finishes.</summary>
    public bool WasSuccessful { get; protected set; }

    public abstract void Start();
    public abstract bool UpdateChallenge(float deltaTime);
    public virtual string GetDisplayName() => GetType().Name;
    public virtual string GetProgressString() => "";
    public virtual float RemainingTime => timeRemaining;
}