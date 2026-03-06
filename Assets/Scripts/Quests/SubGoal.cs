using System;
using UnityEngine;

namespace Quests
{
    [Serializable]
    public class SubGoal
    {
        [Header("SubGoal details")]
        public string subName = "Default";
        public string subDescription = "Default description";

        [Header("Goal location")]
        public Vector2 goalPosition = Vector2.zero;
        public bool useRadius = true;
        public float radius = 16;
        public Color circleColor = Color.white;
    }
}