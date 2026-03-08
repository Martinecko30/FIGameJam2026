using System.Collections;
using TMPro;
using UnityEngine;

namespace Ritterkreuz.MainMenu
{
    public class MenuFSMButton : MonoBehaviour
    {
        [SerializeField] protected MainMenuStateMachine.WindowState stateToChangeTo;
        [SerializeField] float fontSizeAfterIncrease = 60f;
        [SerializeField] float timeToFullyIncrease = 0.1f;
        float originalFontSize;
        float incrementToResizeEveryFrame;
        protected TextMeshProUGUI tmp;
        protected MainMenuStateMachine fsm;

        protected virtual void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            fsm = GetComponentInParent<MainMenuStateMachine>();
            originalFontSize = tmp.fontSize;
            incrementToResizeEveryFrame = (fontSizeAfterIncrease - originalFontSize) / timeToFullyIncrease;
        }

        public void OnClick()
        {
            fsm.InvokeButtonHitEvent(stateToChangeTo);
        }

        public void IncreaseFontSize()
        {
            StopAllCoroutines();
            StartCoroutine(IncreaseFont());
        }

        public void ResetFontSize()
        {
            StopAllCoroutines();
            StartCoroutine(DecreaseFont());
        }

        IEnumerator IncreaseFont()
        {
            while (tmp.fontSize < fontSizeAfterIncrease)
            {
                tmp.fontSize = Mathf.Min(tmp.fontSize + incrementToResizeEveryFrame * Time.deltaTime, fontSizeAfterIncrease);
                yield return null;
            }
        }

        IEnumerator DecreaseFont()
        {
            while (tmp.fontSize > originalFontSize)
            {
                tmp.fontSize = Mathf.Max(tmp.fontSize - incrementToResizeEveryFrame * Time.deltaTime, originalFontSize);
                yield return null;
            }
        }
    }
}
