using Ink.Runtime;
using JetBrains.Annotations;
using Managers;
using Quests;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    [RequireComponent(typeof(Collider))]
    public class Dialog : MonoBehaviour
    {
        public TextAsset InkAsset;

        [SerializeField] [CanBeNull] private SubGoal finishesGoal;
        
        private Story inkStory;

        public UnityEvent finishesDialog;

        private void Awake()
        {
            inkStory = new Story(InkAsset.text);
        }

        public void ContinueDialogue()
        {
            if (!inkStory.canContinue)
            {
                EndDialogue();
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
            }
        }

        private void ChooseOptions(int option)
        {
            inkStory.ChooseChoiceIndex(option);
            ContinueDialogue();
        }

        private void EndDialogue()
        {
            finishesDialog?.Invoke();
            if (finishesGoal != null)
                QuestManager.Instance.CompleteSubGoal(finishesGoal);
         
            DialogueManager.Instance.EndDialogue();
        }
    }
}