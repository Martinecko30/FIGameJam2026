using UnityEngine;
using FPSDemo.Weapons;

namespace FPSDemo.Player
{
    public class PlayerNoiseMaker : MonoBehaviour
    {
        // ========================================================= INSPECTOR FIELDS

        [SerializeField] private Player _player;
        [SerializeField] private PlayerWeaponController _weaponController;

        [Header("Movement noise levels (0-1)")]
        [SerializeField] private float _walkNoiseLevel = 0.4f;
        [SerializeField] private float _sprintNoiseLevel = 1f;
        [SerializeField] private float _crouchWalkNoiseLevel = 0.1f;

        [Header("Weapon noise")]
        [SerializeField] private float _weaponNoiseDuration = 3f;


        // ========================================================= PROPERTIES

        public static PlayerNoiseMaker Instance { get; private set; }

        /// <summary>Movement noise level from 0 (silent) to 1 (maximum).</summary>
        public float MovementNoiseLevel { get; private set; }

        /// <summary>How far the last weapon use can be heard. 0 when no recent weapon use.</summary>
        public float WeaponNoiseRange { get; private set; }

        public Vector3 NoisePosition => transform.position;


        // ========================================================= UNITY METHODS

        private void OnValidate()
        {
            if (_player == null)
                _player = GetComponent<Player>();
            if (_weaponController == null)
                _weaponController = GetComponent<PlayerWeaponController>();
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            MovementNoiseLevel = 0f;
            if (_player.IsMoving())
            {
                if (_player.IsSprinting)
                    MovementNoiseLevel = _sprintNoiseLevel;
                else if (_player.IsCrouching)
                    MovementNoiseLevel = _crouchWalkNoiseLevel;
                else
                    MovementNoiseLevel = _walkNoiseLevel;
            }

            var timeSinceLastShot = Time.time - _player.ThisTarget.LastTimeFired;
            WeaponNoiseRange = timeSinceLastShot < _weaponNoiseDuration
                ? _weaponController.EquippedWeapon.gunShotDistanceToAlert
                : 0f;
        }
    }
}
