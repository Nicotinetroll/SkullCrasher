using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using System.Collections;
using UnityEngine;

namespace OctoberStudio.Abilities
{
    /// <summary>
    /// Ability that spawns rune mines on the ground. When triggered, they deal damage to enemies.
    /// </summary>
    public class MagicRuneAbilityBehavior : AbilityBehavior<MagicRuneAbilityData, MagicRuneAbilityLevel>
    {
        private static readonly int MAGIC_RUNE_SETUP_HASH = "Magic Rune Setup".GetHashCode();

        [Tooltip("Rune Mine Prefab")]
        [SerializeField] GameObject runeMinePrefab;
        public GameObject RuneMinePrefab => runeMinePrefab;

        private PoolComponent<MagicRuneMineBehavior> minePool;

        private void Awake()
        {
            minePool = new PoolComponent<MagicRuneMineBehavior>("Magic Rune Ability", RuneMinePrefab, 6);
        }

        public override void Init(AbilityData data, int stageId)
        {
            base.Init(data, stageId);

            StartCoroutine(AbilityCoroutine());
        }

        private IEnumerator AbilityCoroutine()
        {
            while (true)
            {
                for (int i = 0; i < AbilityLevel.MinesCount; i++)
                {
                    var mine = minePool.GetEntity();

                    mine.transform.position = PlayerBehavior.CenterPosition + Random.onUnitSphere.XY().normalized * AbilityLevel.MineSpawnRadius * Random.Range(0.9f, 1.1f);
                    mine.SetData(AbilityLevel);

                    // ✅ nastav AbilityType pre damage tracking
                    mine.SourceAbilityType = AbilityType.MagicRune;
                }

                GameController.AudioManager.PlaySound(MAGIC_RUNE_SETUP_HASH);

                yield return new WaitForSeconds(AbilityLevel.AbilityCooldown * PlayerBehavior.Player.CooldownMultiplier);
            }
        }
    }
}