using Ink.Runtime;
using Managers;
using UnityEngine;

namespace Dialogue
{
    [RequireComponent(typeof(Collider))]
    public class Dialog : MonoBehaviour
    {
        public TextAsset InkAsset;

        private Story inkStory;

        private void Awake()
        {
            inkStory = new Story(InkAsset.text);
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
            }
        }

        private void ChooseOptions(int option)
        {
            inkStory.ChooseChoiceIndex(option);
            ContinueDialogue();
        }
    }
}