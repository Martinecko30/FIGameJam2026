using FPSDemo.Target;
using FPSDemo.Weapons;
using UnityEngine;
using System;

namespace FPSDemo.Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        // ========================================================= INSPECTOR FIELDS

        [SerializeField] private Weapon _equippedWeapon;
        [SerializeField] private LayerMask _shotLayerMask;
        [SerializeField] private int _ragdollBodyLayerIndex;

        [Tooltip("Multiplier to apply to player speed when aiming.")] [SerializeField]
        private float _aimMultiplier = 0.4f;

        [SerializeField] private Transform _bulletSpawnPoint;

        [SerializeField] private Player _player;


        // ========================================================= PRIVATE FIELDS

        private bool _weaponAtTheReady = false;

        private float _currentOverallAngleSpread;
        private float _angleSpreadFromShooting = 0;
        private float _maxAngleSpread = 15f;
        private float _lastFired = 0.0f;

        // ========================================================= PROPERTIES

        public Weapon EquippedWeapon => _equippedWeapon;

        public bool WeaponAtTheReady
        {
            set => _weaponAtTheReady = value;
            get => _weaponAtTheReady;
        }

        public float CurrentOverallAngleSpread => _currentOverallAngleSpread;

        public Action OnUpdate { get; set; }
        public Action OnFire { get; set; }
        public Action OnReload { get; set; }


        // ========================================================= UNITY METHODS

        private void OnValidate()
        {
            if (_player == null)
            {
                _player = GetComponent<Player>();
            }
        }

        private void Awake()
        {
            InitStartingVariables();
        }

        private void Update()
        {
            if (_player.ThisTarget.IsDead)
            {
                return;
            }

            OnUpdate.Invoke();

            if (ShouldFireTheGun())
            {
                FireInput();
            }

            if (_player.IsAiming == false && _player.InputManager.AimInputAction.IsPressed())
            {
                AimInput(true);
            }
            else if (_player.IsAiming && _player.InputManager.AimInputAction.IsPressed() == false)
            {
                AimInput(false);
            }

            UpdateFiringSpread();
            FocusADS();
        }


        // ========================================================= INIT

        private void InitStartingVariables()
        {
            _lastFired = -_equippedWeapon.fireRate;
        }


        // ========================================================= INPUT TRIGGERS

        public void FireInput()
        {
            if (Time.time > _equippedWeapon.fireRate + _lastFired && !_player.IsAiming)
            {
                _lastFired = Time.time;
                Fire();
            }
        }

        public void AimInput(bool aimIn)
        {
            if (aimIn)
            {
                _player.IsAiming = true;
            }
            else if (_player.IsAiming)
            {
                _player.IsAiming = false;
            }
        }


        // ========================================================= VALIDATORS

        private bool ShouldFireTheGun()
        {
            // Tapping button for semi-auto, holding for full auto and gun pos needs to be either normal, or aiming
            return _player.InputManager.FireInputAction.WasPressedThisFrame();
        }


        // ========================================================= ACTIONS

        private void FocusADS()
        {
            if (_player.IsAiming)
            {
                _player.AimingMultiplier = _aimMultiplier;
            }
            else
            {
                _player.AimingMultiplier = 1.0f;
            }
        }

        private void Fire()
        {
            var maxAngle = _currentOverallAngleSpread;

            if (_player.IsAiming == false)
            {
                maxAngle += _equippedWeapon.defaultHipFireAngleSpread;
            }

            var xAngle = UnityEngine.Random.Range(0, maxAngle);
            var yAngle = UnityEngine.Random.Range(0, maxAngle);

            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                xAngle *= -1f;
            }

            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                yAngle *= -1f;
            }

            _equippedWeapon.Fire(_player.ThisTarget, _bulletSpawnPoint, _shotLayerMask, _ragdollBodyLayerIndex);
            _player.ThisTarget.LastTimeFired = Time.time;
            _angleSpreadFromShooting += _equippedWeapon.angleSpreadPerShot;

            OnFire?.Invoke();
        }


        // ========================================================= TICK

        private void UpdateFiringSpread()
        {
            if (_angleSpreadFromShooting > 0)
            {
                var targetMaxSpreadFromShooting = _equippedWeapon.maxAngleSpreadWhenShooting;
                _angleSpreadFromShooting =
                    Mathf.Clamp(
                        _angleSpreadFromShooting -
                        _equippedWeapon.spreadStabilityGain.Evaluate(Time.time - _lastFired) * Time.deltaTime, 0,
                        targetMaxSpreadFromShooting);
            }

            _currentOverallAngleSpread = Mathf.Clamp(_angleSpreadFromShooting, 0, _maxAngleSpread);
        }
    }
}