using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/New Quest", order = 1)]
    [Serializable]
    public class Quest : ScriptableObject
    {
        [Header("Quest details")]
        public string questName = "Default";
        public string description = "Default description";

        [Header("Next Quest")]
        public Quest nextQuest;

        [Header("Sub Goals")]
        public List<SubGoal> subGoals = new();
    }
}