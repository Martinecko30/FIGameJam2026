using System.Collections.Generic;
using UnityEngine;

namespace FPSDemo.NPC
{
    public enum BarkType { Damage, Death, Investigate, CombatTaunt, HitLanded }

    public class NPCBarkSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Header("Bark Clips")]
        [SerializeField] private AudioClip[] _damageBarks;
        [SerializeField] private AudioClip[] _deathBarks;
        [SerializeField] private AudioClip[] _investigateBarks;
        [SerializeField] private AudioClip[] _combatTauntBarks;
        [SerializeField] private AudioClip[] _hitLandedBarks;

        [Header("Timing")]
        [Tooltip("Minimum seconds before this NPC can bark again.")]
        [SerializeField] private float _perNPCCooldown = 5f;
        [Tooltip("Average seconds between combat taunts.")]
        [SerializeField] private float _combatTauntInterval = 8f;

        private float _npcCooldownUntil;
        private float _nextCombatTauntTime;
        private bool _isInCombat;

        // Shared across all NPC instances so no two NPCs repeat the same bark back-to-back
        private static readonly Dictionary<BarkType, int> _lastPlayedIndex = new()
        {
            { BarkType.Damage,      -1 },
            { BarkType.Death,       -1 },
            { BarkType.Investigate, -1 },
            { BarkType.CombatTaunt, -1 },
            { BarkType.HitLanded,   -1 },
        };

        // Per-category global end times — at most one bark per category playing at a time
        private static readonly Dictionary<BarkType, float> _categoryEndTimes = new()
        {
            { BarkType.Damage,      0f },
            { BarkType.Death,       0f },
            { BarkType.Investigate, 0f },
            { BarkType.CombatTaunt, 0f },
            { BarkType.HitLanded,   0f },
        };

        public void SetInCombat(bool value)
        {
            if (_isInCombat == value) return;
            _isInCombat = value;
            if (value)
                _nextCombatTauntTime = Time.time + Random.Range(_combatTauntInterval * 0.5f, _combatTauntInterval);
        }

        private void Update()
        {
            if (_isInCombat && Time.time >= _nextCombatTauntTime)
            {
                TriggerBark(BarkType.CombatTaunt);
                _nextCombatTauntTime = Time.time + Random.Range(_combatTauntInterval * 0.5f, _combatTauntInterval);
            }
        }

        public void TriggerBark(BarkType type)
        {
            var clips = GetClips(type);
            if (clips == null || clips.Length == 0) return;

            var clip = clips[PickIndex(type, clips.Length)];

            if (type == BarkType.Death)
            {
                // Death always plays — cuts off everything on this NPC
                _audioSource.Stop();
                _categoryEndTimes[type] = Time.time + clip.length;
                _npcCooldownUntil = Time.time + _perNPCCooldown;
                _audioSource.PlayOneShot(clip);
                return;
            }

            if (type == BarkType.HitLanded)
            {
                // HitLanded cuts through low-priority barks but won't override another HitLanded
                if (Time.time < _categoryEndTimes[type]) return;
                _audioSource.Stop();
                _categoryEndTimes[type] = Time.time + clip.length;
                _npcCooldownUntil = Time.time + _perNPCCooldown;
                _audioSource.PlayOneShot(clip);
                return;
            }

            // Low-priority barks: respect per-NPC cooldown and own category slot
            if (Time.time < _npcCooldownUntil) return;
            if (Time.time < _categoryEndTimes[type]) return;

            _categoryEndTimes[type] = Time.time + clip.length;
            _npcCooldownUntil = Time.time + _perNPCCooldown;
            _audioSource.PlayOneShot(clip);
        }

        private int PickIndex(BarkType type, int count)
        {
            if (count == 1) return 0;
            int last = _lastPlayedIndex[type];
            int index = Random.Range(0, count - 1);
            if (last >= 0 && index >= last) index++;
            _lastPlayedIndex[type] = index;
            return index;
        }

        private AudioClip[] GetClips(BarkType type) => type switch
        {
            BarkType.Damage      => _damageBarks,
            BarkType.Death       => _deathBarks,
            BarkType.Investigate => _investigateBarks,
            BarkType.CombatTaunt => _combatTauntBarks,
            BarkType.HitLanded   => _hitLandedBarks,
            _                    => null
        };
    }
}
