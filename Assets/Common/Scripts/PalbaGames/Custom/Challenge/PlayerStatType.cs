namespace PalbaGames.Challenges
{
    /// <summary>
    /// Enum for all modifiable player stats that can be affected by penalties/buffs.
    /// Maps directly to PlayerStatModifier properties and critical strike stats.
    /// </summary>
    public enum PlayerStatType
    {
        DamageMultiplier,
        SpeedMultiplier,
        MagnetRadiusMultiplier,
        XpMultiplier,
        CooldownMultiplier,
        DamageReductionExtraPercent,
        ProjectileSpeedMultiplier,
        SizeMultiplier,
        DurationMultiplier,
        GoldMultiplier,
        CriticalChance,
        CriticalMultiplierMin,
        CriticalMultiplierMax
    }
}