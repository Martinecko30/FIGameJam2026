using System.Collections.Generic;
using UnityEngine;

namespace Areas
{
    [RequireComponent(typeof(Collider))]
    public class Area : MonoBehaviour
    {
        public bool IsActive { get; private set; } = false;
        public Collider AreaTrigger { get; private set; }

        [SerializeField] private List<Collider> areaBounds = new();

        public void Activate()
        {
            foreach (var wall in areaBounds)
            {
                wall.gameObject.SetActive(true);
            } 
            
            IsActive = true;
        }

        public void Deactivate()
        {
            foreach (var wall in areaBounds)
            {
                wall.gameObject.SetActive(false);
            } 
            
            IsActive = false;
        }
    }
}