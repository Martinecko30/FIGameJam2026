using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FPSDemo.Target;

namespace FPSDemo.UI
{
    public class DamageVignette : MonoBehaviour
    {
        [SerializeField] private HealthSystem _healthSystem;
        [SerializeField] private Image _vignetteImage;

        [Header("Flash")]
        [SerializeField] private float _flashPeakAlpha = 0.6f;
        [SerializeField] private float _flashFadeDuration = 0.5f;

        [Header("Low Health")]
        [SerializeField] private float _lastHitMinAlpha = 0.2f;

        private Coroutine _fadeCoroutine;

        private void Start()
        {
            // Shader defines the red color; we only drive the alpha via vertex color
            _vignetteImage.color = new Color(1f, 1f, 1f, 0f);
            _healthSystem.OnDamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken(Vector3 _, HumanTarget __)
        {
            bool isLastHit = _healthSystem.HitCount >= _healthSystem.HitsToKill - 1;
            float targetAlpha = isLastHit ? _lastHitMinAlpha : 0f;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FlashAndFade(_flashPeakAlpha, targetAlpha, _flashFadeDuration));
        }

        private IEnumerator FlashAndFade(float peakAlpha, float targetAlpha, float duration)
        {
            SetAlpha(peakAlpha);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(peakAlpha, targetAlpha, elapsed / duration);
                SetAlpha(alpha);
                yield return null;
            }

            SetAlpha(targetAlpha);
            _fadeCoroutine = null;
        }

        private void SetAlpha(float alpha)
        {
            Color c = _vignetteImage.color;
            c.a = alpha;
            _vignetteImage.color = c;
        }

        private void OnDestroy()
        {
            _healthSystem.OnDamageTaken -= OnDamageTaken;
        }
    }
}
