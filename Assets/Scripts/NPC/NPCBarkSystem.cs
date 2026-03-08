using UnityEngine;

namespace FPSDemo.NPC
{
    public enum BarkType { Damage, Death, Investigate, CombatTaunt }

    public class NPCBarkSystem : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Header("Bark Clips")]
        [SerializeField] private AudioClip[] _damageBarks;
        [SerializeField] private AudioClip[] _deathBarks;
        [SerializeField] private AudioClip[] _investigateBarks;
        [SerializeField] private AudioClip[] _combatTauntBarks;

        [Header("Timing")]
        [Tooltip("Minimum seconds before this NPC can bark again.")]
        [SerializeField] private float _perNPCCooldown = 5f;
        [Tooltip("Average seconds between combat taunts.")]
        [SerializeField] private float _combatTauntInterval = 8f;

        private float _npcCooldownUntil;
        private float _nextCombatTauntTime;
        private bool _isInCombat;

        public void SetInCombat(bool value)
        {
            if (_isInCombat == value) return;
            _isInCombat = value;
            Debug.Log($"[BarkSystem] {name} combat state → {value}");
            if (value)
                _nextCombatTauntTime = Time.time + Random.Range(_combatTauntInterval * 0.5f, _combatTauntInterval);
        }

        private void Update()
        {
            if (_isInCombat && Time.time >= _nextCombatTauntTime)
            {
                Debug.Log($"[BarkSystem] {name} attempting combat taunt");
                TriggerBark(BarkType.CombatTaunt);
                _nextCombatTauntTime = Time.time + Random.Range(_combatTauntInterval * 0.5f, _combatTauntInterval);
            }
        }

        public void TriggerBark(BarkType type)
        {
            var clips = GetClips(type);
            if (clips == null || clips.Length == 0) return;

            var clip = clips[Random.Range(0, clips.Length)];

            if (type == BarkType.Death)
            {
                // Death always plays — cut off any current bark and interrupt cooldowns
                _audioSource.Stop();
                BarkCoordinator.TryReserve(clip.length);
                _audioSource.PlayOneShot(clip);
                return;
            }

            if (Time.time < _npcCooldownUntil) return;
            if (!BarkCoordinator.TryReserve(clip.length)) return;

            _npcCooldownUntil = Time.time + _perNPCCooldown;
            _audioSource.PlayOneShot(clip);
        }

        private AudioClip[] GetClips(BarkType type) => type switch
        {
            BarkType.Damage      => _damageBarks,
            BarkType.Death       => _deathBarks,
            BarkType.Investigate => _investigateBarks,
            BarkType.CombatTaunt => _combatTauntBarks,
            _                    => null
        };
    }
}
