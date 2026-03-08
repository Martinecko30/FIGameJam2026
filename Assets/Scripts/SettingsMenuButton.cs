using System.Collections;
using UnityEngine;

namespace Ritterkreuz.MainMenu
{
    public class SettingsMenuButton : MenuFSMButton
    {
        [SerializeField] Color colorToSet;
        [SerializeField] float transitionTime = 0.1f;
        Color defaultColor;
        float t = 0.0f;

        protected override void Awake()
        {
            base.Awake();
            defaultColor = tmp.color;
        }

        void OnEnable()
        {
            fsm.SettingsButtonColorCheck += SomeButtonClicked;
        }

        void OnDisable()
        {
            fsm.SettingsButtonColorCheck -= SomeButtonClicked;
        }

        void SomeButtonClicked()
        {
            StopAllCoroutines();
            if (fsm.CurrentState == stateToChangeTo)
            {
                StartCoroutine(ChangeColor());
            }
            else if (tmp.color != defaultColor)
            {
                StartCoroutine(ResetColor());
            }
        }

        IEnumerator ChangeColor()
        {
            while (tmp.color != colorToSet)
            {
                t += Time.deltaTime;
                tmp.color = Color.Lerp(defaultColor, colorToSet, t / transitionTime);
                yield return null;
            }
            t = 0;
        }

        IEnumerator ResetColor()
        {
            while (tmp.color != defaultColor)
            {
                t += Time.deltaTime / transitionTime;
                tmp.color = Color.Lerp(colorToSet, defaultColor, t);
                yield return null;
            }
            t = 0;
        }
    }
}
