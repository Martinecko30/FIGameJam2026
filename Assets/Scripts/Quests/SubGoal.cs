using System;
using UnityEngine;

namespace Quests
{
    [Serializable]
    public enum CompletionType
    {
        Automatic = 0,
        Dialog = 1,
        EnemySlain = 2,
        ItemInteract = 3
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "Sub Goal", menuName = "Quests/New Sub Goal", order = 2)]
    public class SubGoal : ScriptableObject
    {
        [Header("Details")]
        public string subName = "Default";
        public string subDescription = "Default description";

        [Header("Location")]
        public Vector2 goalPosition = Vector2.zero;
        public bool useRadius = true;
        public float radius = 16;
        
        [Header("Completion")]
        public CompletionType completionType = CompletionType.Automatic;
        public bool isNecessary = false;
    }
}