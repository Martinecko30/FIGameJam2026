using UnityEngine;
using UnityEngine.UI;
using FPSDemo.Target;

namespace FPSDemo.UI
{
    public class HealthSlider : MonoBehaviour
    {
        [SerializeField] private HealthSystem _healthSystem;
        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        private void Start()
        {
            _healthSystem.OnDamageTaken += OnDamageTaken;
            UpdateSlider();
        }

        private void OnDamageTaken(Vector3 _, HumanTarget __)
        {
            UpdateSlider();
        }

        private void UpdateSlider()
        {
            _slider.value = 1f - (float)_healthSystem.HitCount / _healthSystem.HitsToKill;
        }

        private void OnDestroy()
        {
            _healthSystem.OnDamageTaken -= OnDamageTaken;
        }
    }
}
