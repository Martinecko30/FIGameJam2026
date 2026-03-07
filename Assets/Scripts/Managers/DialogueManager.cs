using System;
using System.Collections.Generic;
using Core;
using FPSDemo.Input;
using Ink.Runtime;
using JetBrains.Annotations;
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

        [CanBeNull] private Dialogue.Dialogue currentDialogue;
        
        private bool choiceAlreadyChosen = false;

        private void Start()
        {
            InputSystem.onAnyButtonPress.Call(delegate
            {
                if (!Application.isPlaying)
                    return;
                
                if (currentDialogue != null && choices.Count == 0)
                    currentDialogue.ContinueDialogue();
            });
        }

        public void AddChoice(Choice choice, int i, Action<int> callback)
        {
            choiceAlreadyChosen = false;
            var button = Instantiate(prefab, choicesParent);
            
            button.GetComponentInChildren<TMP_Text>().text = choice.text;
            button.gameObject.name = $"Choice Button {i}";
            button.gameObject.SetActive(true);
            choices.Add(button);
            button.onClick.AddListener(() =>
            {
                if (choiceAlreadyChosen)
                    return;
                ClearChoices();
                choiceAlreadyChosen = true;
                
                callback?.Invoke(i);
            });
        }

        public void SetDialogue(string text, Dialogue.Dialogue dialogue)
        {
            currentDialogue = dialogue;
            dialogueText.text = text;
        }
        
        private void OnDestroy()
        {
            ClearChoices();
        }

        private void ClearChoices()
        {
            Debug.Log(choices);
            foreach (var choiceButton in choices)
                Destroy(choiceButton.gameObject);
            choices.Clear();
        }

        public void EndDialogue()
        {
            ClearChoices();
            dialogueText.text = "No dialogue!";
            dialogueText.gameObject.SetActive(false);
            choicesParent.gameObject.SetActive(false);
        }
    }
}