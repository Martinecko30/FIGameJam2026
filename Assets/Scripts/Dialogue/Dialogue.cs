using System;
using Ink.Runtime;
using Managers;
using Quests;
using UnityEngine;

namespace Dialogue
{
    public class Dialogue : MonoBehaviour
    {
        public TextAsset InkAsset;

        private Story inkStory;
        
        // TODO: Remove quest
        [SerializeField] private Quest _quest;

        private void Awake()
        {
            inkStory = new Story(InkAsset.text);
        }

        private void Start()
        {
            QuestManager.Instance.SetQuest(_quest);
        }

        public void StartDialogue()
        {
            ContinueDialogue();
        }

        public void ContinueDialogue()
        {
            if (!inkStory.canContinue)
            {
                DialogueManager.Instance.EndDialogue();
                return;
            }
                
            DialogueManager.Instance.SetDialogue(inkStory.Continue(), this);
    
            if (inkStory.currentChoices.Count > 0)
            {
                DisplayChoices();
            }
        }

        private void DisplayChoices()
        {
            for (int i = 0; i < inkStory.currentChoices.Count; ++i) 
            {
                Choice choice = inkStory.currentChoices[i];
                DialogueManager.Instance.AddChoice(choice, i, ChooseOptions);
        
                Debug.Log($"Choice {i + 1}. {choice.text}"); 
            }
        }

        public void ChooseOptions(int option)
        {
            inkStory.ChooseChoiceIndex(option);
            ContinueDialogue();
        }
    }
}