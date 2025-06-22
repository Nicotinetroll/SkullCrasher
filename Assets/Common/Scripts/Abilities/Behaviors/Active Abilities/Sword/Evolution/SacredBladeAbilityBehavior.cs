using OctoberStudio.Pool;
using OctoberStudio.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PalbaGames;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Sacred Blade ability that spawns slashes and projectile waves.
    /// </summary>
    public class SacredBladeAbilityBehavior : AbilityBehavior<SacredBladeAbilityData, SacredBladeAbilityLevel>
    {
        public static readonly int SACRED_BLADE_ATTACK_HASH = "Sacred Blade Attack".GetHashCode();

        [SerializeField] GameObject slashPrefab;
        public GameObject SlashPrefab => slashPrefab;

        [SerializeField] GameObject wavePrefab;
        public GameObject WavePrefab => wavePrefab;

        private PoolComponent<SwordSlashBehavior> slashPool;
        private PoolComponent<SimplePlayerProjectileBehavior> wavePool;

        private List<SwordSlashBehavior> slashes = new();
        private List<SimplePlayerProjectileBehavior> waves = new();

        [SerializeField] List<Transform> shashDirections;

        private Coroutine abilityCoroutine;

        private float AbilityCooldown => AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier;

        private void Awake()
        {
            slashPool = new PoolComponent<SwordSlashBehavior>("Blade Slash", SlashPrefab, 6);
            wavePool = new PoolComponent<SimplePlayerProjectileBehavior>("Blade Wave", WavePrefab, 12);
        }

        protected override void SetAbilityLevel(int stageId)
        {
            base.SetAbilityLevel(stageId);

            if (abilityCoroutine != null) Disable();

            abilityCoroutine = StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < AbilityLevel.SlashesCount; i++)
                {
                    var slash = slashPool.GetEntity();
                    slash.transform.position = PlayerBehavior.CenterPosition;
                    slash.transform.rotation = Quaternion.FromToRotation(Vector2.right, PlayerBehavior.Player.LookDirection) * shashDirections[i].localRotation;

                    slash.DamageMultiplier = AbilityLevel.Damage;
                    slash.KickBack = false;
                    slash.Size = AbilityLevel.SlashSize;

                    // ⬇️ Set damage source
                    slash.SourceAbilityType = AbilityType.SacredBlade;

                    slash.Init();
                    slash.onFinished += OnProjectileFinished;
                    slashes.Add(slash);

                    var wave = wavePool.GetEntity();
                    wave.DamageMultiplier = AbilityLevel.WaveDamage;
                    wave.KickBack = false;

                    // ⬇️ Set damage source
                    wave.SourceAbilityType = AbilityType.SacredBlade;

                    wave.Init(
                        PlayerBehavior.CenterPosition,
                        Quaternion.FromToRotation(Vector2.right, PlayerBehavior.Player.LookDirection) * shashDirections[i].localRotation * Vector2.right
                    );

                    wave.ScaleRotatingPart(Vector2.up, Vector2.one).SetEasing(EasingType.SineOut);
                    wave.onFinished += OnWaveFinished;
                    waves.Add(wave);

                    GameController.AudioManager.PlaySound(SACRED_BLADE_ATTACK_HASH);

                    yield return new WaitForSeconds(AbilityLevel.TimeBetweenSlashes * PlayerBehavior.Player.CooldownMultiplier);
                }

                yield return new WaitForSeconds(
                    AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier
                    - AbilityLevel.TimeBetweenSlashes * PlayerBehavior.Player.CooldownMultiplier * AbilityLevel.SlashesCount
                );
            }
        }

        private void OnProjectileFinished(SwordSlashBehavior slash)
        {
            slash.onFinished -= OnProjectileFinished;
            slashes.Remove(slash);
        }

        private void OnWaveFinished(SimplePlayerProjectileBehavior wave)
        {
            wave.onFinished -= OnWaveFinished;
            waves.Remove(wave);
        }

        private void Disable()
        {
            foreach (var slash in slashes) slash.Disable();
            foreach (var wave in waves) wave.Clear();

            slashes.Clear();
            waves.Clear();

            StopCoroutine(abilityCoroutine);
        }

        public override void Clear()
        {
            Disable();
            slashPool.Destroy();
            wavePool.Destroy();
            base.Clear();
        }
    }
}
