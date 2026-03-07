using System;
using System.Collections.Generic;
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
        [SerializeField] private TMP_Text subGoalPrefab;
        
        private Quest ActiveQuest;

        public Action<Quest> OnQuestComplete;

        public void SetQuest(Quest quest)
        {
            ActiveQuest = quest;
            mainGoalText.text = quest.questName;

            foreach (var subGoal in quest.subGoals)
            {
                CreateSubGoal(subGoal);
            }
        }

        private void CreateSubGoal(SubGoal goal)
        {
            var subGoal = Instantiate(subGoalPrefab, goalsParent);
            subGoal.text = goal.subDescription;
        }
    }
}