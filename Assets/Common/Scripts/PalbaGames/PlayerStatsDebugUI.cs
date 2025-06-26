using UnityEngine;
using TMPro;
using OctoberStudio;
using System.Collections.Generic;

namespace PalbaGames
{
    /// <summary>
    /// Tracks and displays all player stats with color highlighting for changed values only.
    /// Attach to UI TextMeshProUGUI element.
    /// </summary>
    public class PlayerStatsDebugUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float highlightDuration = 5f;

        private PlayerBehavior player;
        private PlayerStatModifier modifier;
        private PlayerBehavior_Extended critSource;

        private TrackedValue dmg, spd, xp, cd, ps, sz, dur, gold, magnet, dmgReduction;
        private TrackedValue critChance, critMultMin, critMultMax;

        private float prevHP;
        private string hpColor = "#FFFFFF";
        private HealthbarBehavior healthbar;

        private void Start()
        {
            player = PlayerBehavior.Player ?? FindObjectOfType<PlayerBehavior>();
            modifier = player != null ? player.GetComponent<PlayerStatModifier>() : null;
            critSource = player != null ? player.GetComponent<PlayerBehavior_Extended>() : null;
            healthbar = player != null ? player.GetComponentInChildren<HealthbarBehavior>() : null;

            if (text == null || player == null || healthbar == null)
            {
                Debug.LogError("[PlayerStatsDebugUI] Missing references.");
                enabled = false;
                return;
            }

            dmg = new(player.Damage, highlightDuration);
            spd = new(player.Speed, highlightDuration);
            xp = new(player.XPMultiplier, highlightDuration);
            cd = new(player.CooldownMultiplier, highlightDuration);
            ps = new(player.ProjectileSpeedMultiplier, highlightDuration);
            sz = new(player.SizeMultiplier, highlightDuration);
            dur = new(player.DurationMultiplier, highlightDuration);
            gold = new(player.GoldMultiplier, highlightDuration);
            magnet = new(player.MagnetRadiusSqr, highlightDuration);
            dmgReduction = new(player.DamageReductionMultiplier, highlightDuration);

            prevHP = healthbar != null ? healthbar.HP : 0;
        }

        private void Update()
        {
            if (player == null || healthbar == null) return;

            dmg.Check(player.Damage);
            spd.Check(player.Speed);
            xp.Check(player.XPMultiplier);
            cd.Check(player.CooldownMultiplier);
            ps.Check(player.ProjectileSpeedMultiplier);
            sz.Check(player.SizeMultiplier);
            dur.Check(player.DurationMultiplier);
            gold.Check(player.GoldMultiplier);
            magnet.Check(player.MagnetRadiusSqr);
            dmgReduction.Check(player.DamageReductionMultiplier);

            float maxHP = healthbar.MaxHP;
            float currentHP = healthbar.HP;

            if (!Mathf.Approximately(currentHP, prevHP))
            {
                hpColor = currentHP > prevHP ? "#00FF00" : "#FF0000";
                prevHP = currentHP;
                CancelInvoke(nameof(ResetHPColor));
                Invoke(nameof(ResetHPColor), highlightDuration);
            }

            string critStats = "";
            if (critSource != null)
            {
                if (critChance == null) critChance = new(critSource.criticalChance, highlightDuration);
                if (critMultMin == null) critMultMin = new(critSource.criticalMultiplierMin, highlightDuration);
                if (critMultMax == null) critMultMax = new(critSource.criticalMultiplierMax, highlightDuration);

                critChance.Check(critSource.criticalChance);
                critMultMin.Check(critSource.criticalMultiplierMin);
                critMultMax.Check(critSource.criticalMultiplierMax);

                critStats =
                    critChance.FormatValueOnly("Crit Chance", "%") +
                    critMultMin.FormatValueOnly("Crit xMin") +
                    critMultMax.FormatValueOnly("Crit xMax");
            }

            text.text =
                "<b>PLAYER STATS</b>\n" +
                dmg.FormatValueOnly("Damage") +
                spd.FormatValueOnly("Speed") +
                $"HP: <color={hpColor}>{maxHP:F0}/{currentHP:F0}</color>\n" +
                dmgReduction.FormatValueOnly("Dmg Reduction", "", 2) +
                xp.FormatValueOnly("XP Multiplier") +
                cd.FormatValueOnly("Cooldown") +
                ps.FormatValueOnly("Projectile Speed") +
                sz.FormatValueOnly("Size") +
                dur.FormatValueOnly("Duration") +
                gold.FormatValueOnly("Gold") +
                magnet.FormatValueOnly("Magnet Radius²") +
                critStats;
        }

        private void ResetHPColor()
        {
            hpColor = "#FFFFFF";
        }
    }

    public class TrackedValue
    {
        private float current;
        private float lastChangeTime = -999f;
        private float highlightDuration;

        public TrackedValue(float initial, float durationSeconds)
        {
            current = initial;
            highlightDuration = durationSeconds;
        }

        public bool Check(float newValue)
        {
            if (!Mathf.Approximately(current, newValue))
            {
                current = newValue;
                lastChangeTime = Time.time;
                return true;
            }
            return false;
        }

        public string ColorTag => (Time.time - lastChangeTime < highlightDuration) ? "#00FF00" : "#FFFFFF";

        public string FormatValueOnly(string label, string suffix = "", int decimals = 2)
        {
            return $"{label}: <color={ColorTag}>{current.ToString($"F{decimals}")}{suffix}</color>\\n";
        }
    }
} 
