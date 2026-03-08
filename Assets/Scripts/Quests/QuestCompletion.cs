using System.Collections.Generic;
using FPSDemo.NPC;

namespace Quests
{
    public class QuestCompletion
    {
        public Quest ActiveQuest;

        public HashSet<SubGoal> CompletedGoals { get; } = new();

        public QuestCompletion(Quest quest)
        {
            ActiveQuest = quest;
            
            GuardRegistry.OnGuardUnregistered += npc =>
            {
                if (GuardRegistry.GetAlertedGuards().Count == 0)
                    CompleteSlainSubGoal();
            };
        }

        public void CompleteGoal(SubGoal goal)
        {
            CompletedGoals.Add(goal);
        }

        public bool AllNecessaryCompleted()
        {
            bool completed = true;
            foreach (SubGoal goal in CompletedGoals)
            {
                completed &= goal.isNecessary;
            }
            
            return completed || CompletedGoals.Count == ActiveQuest.subGoals.Count;
        }

        private void CompleteSlainSubGoal()
        {
            foreach (SubGoal goal in ActiveQuest.subGoals)
            {
                if (goal.completionType == CompletionType.EnemySlain)
                {
                    CompleteGoal(goal);
                    return;
                }
            }
        }
    }
}