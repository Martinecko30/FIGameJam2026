using System;
using Ink.Runtime;
using Managers;
using UnityEngine;

namespace Dialogue
{
    public class Dialogue : MonoBehaviour
    {
        public TextAsset InkAsset;

        [SerializeField] Story inkStory;

        private void Awake()
        {
            inkStory = new Story(InkAsset.text);
        }

        private void Start()
        {
            // TODO: Remove
            StartDialogue();
        }

        public void StartDialogue()
        {
            ContinueDialogue();
        }

        public void ContinueDialogue()
        {
            if (inkStory.canContinue) {
                DialogueManager.Instance.SetDialogue(inkStory.Continue(), ContinueDialogue);
            }
            else if(inkStory.currentChoices.Count > 0)
            {
                for (int i = 0; i < inkStory.currentChoices.Count; ++i) {
                    Choice choice = inkStory.currentChoices [i];
                    DialogueManager.Instance.AddChoice(choice, i, ChooseOptions);
                    Debug.Log("Choice " + (i + 1) + ". " + choice.text);
                }
            }
        }

        public void ChooseOptions(int option)
        {
            inkStory.ChooseChoiceIndex(option);
            ContinueDialogue();
        }
    }
}