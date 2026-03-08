using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Areas
{
    [RequireComponent(typeof(Collider))]
    public class Area : MonoBehaviour
    {
        public bool IsActive { get; private set; } = false;
        public Collider AreaTrigger { get; private set; }

        [SerializeField] private List<Collider> areaBounds = new();
        

        private void Start()
        {
            Deactivate();
        }

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SetArea(this);
            }
        }
    }
}