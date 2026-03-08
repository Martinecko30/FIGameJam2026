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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        private List<Button> choices = new();
        [SerializeField] private Transform choicesParent;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Button prefab;

        [CanBeNull] private Dialogue.Dialog _currentDialog;
        
        public Action DialogFinished;
        
        private bool choiceAlreadyChosen = false;

        private void Start()
        {
            SceneManager.sceneLoaded += (_, _) => Initialize();
            Initialize();
        }

        public void Initialize()
        {
            choicesParent = GameObject.Find("DialogueOptions").transform;
            dialogueText = GameObject.Find("DialogueText").GetComponent<TextMeshProUGUI>();
            prefab = Resources.Load<Button>("Prefabs/ChoiceButtonPrefab");
            
            InputSystem.onAnyButtonPress.Call(delegate
            {
                if (!Application.isPlaying)
                    return;
                
                if (_currentDialog != null && choices.Count == 0)
                    _currentDialog.ContinueDialogue();
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

        public void SetDialogue(string text, Dialogue.Dialog dialog)
        {
            _currentDialog = dialog;
            dialogueText.text = text;
        }
        
        private void OnDestroy()
        {
            ClearChoices();
        }

        private void ClearChoices()
        {
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
            
            DialogFinished?.Invoke();
        }
    }
}