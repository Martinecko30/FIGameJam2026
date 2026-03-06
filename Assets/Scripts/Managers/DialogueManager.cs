using System;
using System.Collections.Generic;
using Core;
using FPSDemo.Input;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private List<Button> choices = new();
        [SerializeField] private Transform choicesParent;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Button prefab;
        
        public void AddChoice(Choice choice, int i, Action<int> callback)
        {
            var button = Instantiate(prefab, choicesParent);
            
            button.GetComponentInChildren<TMP_Text>().text = choice.text;
            button.gameObject.name = $"Choice Button {i}";
            button.gameObject.SetActive(true);
            choices.Add(button);
            button.onClick.AddListener(() =>
            {
                foreach (var choiceButton in choices)
                    Destroy(choiceButton.gameObject);
                choices.Clear();
                
                callback?.Invoke(i);
            });
        }

        public void SetDialogue(string dialogue, Action callback)
        {
            dialogueText.text = dialogue;
            
            if (Application.isPlaying)
                InputSystem.onAnyButtonPress.Call(_ => callback?.Invoke());
        }
    }
}