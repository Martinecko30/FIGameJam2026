using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Quests;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class QuestManager : Singleton<QuestManager>
    {
        [SerializeField] private Transform goalsParent;
        [SerializeField] private TMP_Text mainGoalText;
        [SerializeField] private Transform subGoalPrefab;
        
        private QuestCompletion questCompletion;

        public Quest ActiveQuest => questCompletion.ActiveQuest;

        public Action<Quest> OnQuestComplete;
        

        public void SetQuest(Quest quest)
        {
            questCompletion = new QuestCompletion(quest);
            ShowQuest(quest);
        }

        private void ShowQuest(Quest quest)
        {
            ClearQuest();
            mainGoalText.text = quest.questName;

            foreach (var subGoal in quest.subGoals)
            {
                if (questCompletion.CompletedGoals.Contains(subGoal))
                    continue;
                CreateSubGoal(subGoal);
            }
        }

        private void CreateSubGoal(SubGoal goal)
        {
            var subGoal = Instantiate(subGoalPrefab, goalsParent);
            subGoal.GetChild(0).GetComponent<TMP_Text>().text = goal.subName;
            subGoal.GetChild(1).GetComponent<TMP_Text>().text = goal.subDescription;
        }

        private void ClearQuest()
        {
            foreach (Transform child in goalsParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void CompleteQuest()
        {
            if (ActiveQuest.nextQuest == null)
                return;
            
            OnQuestComplete?.Invoke(ActiveQuest);
            
            SetQuest(ActiveQuest.nextQuest);
        }

        public void CompleteSubGoal(SubGoal subGoal)
        {
            questCompletion.CompleteGoal(subGoal);
            ShowQuest(ActiveQuest);

            if (questCompletion.AllNecessaryCompleted())
                CompleteQuest();
        }
    }
}