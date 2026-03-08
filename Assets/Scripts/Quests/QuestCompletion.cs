using System.Collections.Generic;

namespace Quests
{
    public class QuestCompletion
    {
        public Quest ActiveQuest;

        public HashSet<SubGoal> CompletedGoals { get; } = new();

        public QuestCompletion(Quest quest)
        {
            ActiveQuest = quest;
        }

        public void CompleteGoal(SubGoal goal)
        {
            CompletedGoals.Add(goal);
        }
    }
}