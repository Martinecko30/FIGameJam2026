using System;
using UnityEngine;

namespace Quests
{
    [Serializable]
    public enum CompletionType
    {
        Automatic = 0,
        Dialog = 1,
        EnemySlain = 2
    }
    
    [Serializable]
    public class SubGoal
    {
        [Header("Details")]
        public string subName = "Default";
        public string subDescription = "Default description";

        [Header("Location")]
        public Vector2 goalPosition = Vector2.zero;
        public bool useRadius = true;
        public float radius = 16;
        public Color circleColor = Color.white;
        
        [Header("Completion")]
        public CompletionType completionType = CompletionType.Automatic;
    }
}