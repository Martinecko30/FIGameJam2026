
using FluidHTN;
using FPSDemo.NPC.FSMs;
using FPSDemo.NPC.FSMs.WeaponStates;
using FPSDemo.NPC.Sensors;
using FPSDemo.Target;
using UnityEngine;

namespace FPSDemo.NPC
{
	[RequireComponent(typeof(HumanTarget), typeof(ThirdPersonController))]
	public class NPC : MonoBehaviour
	{
        // ========================================================= INSPECTOR FIELDS

        [SerializeField] private Animator _animator;
		[SerializeField] private Vector3 _velocity;
        [SerializeField] private NPCSettings _settings;
        [SerializeField] private ThirdPersonController _controller;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private NPCBarkSystem _barkSystem;


        // ========================================================= PRIVATE FIELDS
        
        private AIContext _context;
        private SensorySystem _sensory;
        private Domain<AIContext> _domain;
        private Planner<AIContext> _planner;

        private WeaponFsm _weaponFsm;
        private HealthSystem _healthSystem;

        // ========================================================= PUBLIC PROPERTIES

        public ThirdPersonController Controller => _controller;
        public AIContext Context => _context;
        public PatrolPath PatrolPath => _patrolPath;

        // ========================================================= UNITY METHODS

        private void Awake()
        {
            if (_controller == null)
            {
                _controller = GetComponent<ThirdPersonController>();
            }

            _context = new AIContext(this, GetComponent<HumanTarget>());
            _sensory = new SensorySystem(this);
            _planner = new Planner<AIContext>();
            _domain = _settings.AIDomain.Create();

            _weaponFsm = new WeaponFsm();
        }

		public void Start()
		{
			_context.Init(_settings);

            // Subscribe to damage events
            _healthSystem = GetComponent<HealthSystem>();
            if (_healthSystem != null)
            {
                _healthSystem.OnDamageTaken += OnDamageTaken;
                _healthSystem.OnDeath += OnDeath;
            }

            AlertSystem.OnEnemySpotted += OnEnemySpottedAlert;

            // NPC starts off holding their fire, until the planner decides otherwise.
            _context.SetWeaponState(WeaponStateType.HoldYourFire, EffectType.Permanent);
            _weaponFsm.ChangeState((int)WeaponStateType.HoldYourFire, _context);
        }

        /// <summary>Sends this NPC to investigate a world position without alerting them to the player.</summary>
        public void SendToInvestigate(Vector3 position)
        {
            _context.SetInvestigatePosition(position);
        }

        /// <summary>Immediately fully alerts this NPC to the given target.</summary>
        public void ForceAlert(HumanTarget target)
        {
            if (_context.EnemiesSpecificData.ContainsKey(target))
            {
                _context.SetAwarenessOfThisEnemy(target, _context.AlertAwarenessThreshold);
            }
        }

        private void OnEnemySpottedAlert(HumanTarget spottedTarget, Vector3 reporterPosition, float alertRadius)
        {
            if (Vector3.Distance(transform.position, reporterPosition) > alertRadius) return;
            if (!_context.EnemiesSpecificData.ContainsKey(spottedTarget)) return;

            _context.SetAwarenessOfThisEnemy(spottedTarget, _context.AlertAwarenessThreshold);
        }
        
        private void OnDamageTaken(Vector3 damagePosition, HumanTarget attacker)
        {
            if (attacker != null && _context.EnemiesSpecificData.ContainsKey(attacker))
            {
                if (!_context.HasState(AIWorldState.AwareOfEnemy))
                {
                    _healthSystem.ForceKill();
                    return;
                }

                _context.SetAwarenessOfThisEnemy(attacker, _context.AlertAwarenessThreshold);
            }

            if (_healthSystem.HitCount < _healthSystem.HitsToKill)
                _barkSystem?.TriggerBark(BarkType.Damage);
            _context.RecordDamageAtCurrentPosition();
            _context.SetState(AIWorldState.CurrentPositionCompromised, true, EffectType.Permanent);
        }

        private void OnDeath()
        {
            _barkSystem?.TriggerBark(BarkType.Death);
            if (_barkSystem != null) _barkSystem.enabled = false;
            _controller.Death();
            var visionSensor = GetComponent<VisionSensor>();
            if (visionSensor != null) visionSensor.enabled = false;
            enabled = false;
        }

        public void Bark(BarkType type) => _barkSystem?.TriggerBark(type);

        public void Update()
        {
			_sensory.Tick(_context);
            _planner.Tick(_domain, _context);

            _weaponFsm.Tick(_context);
            _barkSystem?.SetInCombat(_context.HasState(AIWorldState.AwareOfEnemy));
        }
        
        private void OnDestroy()
        {
            if (_healthSystem != null)
            {
                _healthSystem.OnDamageTaken -= OnDamageTaken;
                _healthSystem.OnDeath -= OnDeath;
            }

            AlertSystem.OnEnemySpotted -= OnEnemySpottedAlert;
        }
    }
}
