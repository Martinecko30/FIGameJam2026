using System;
using Areas;
using Core;
using FPSDemo.Target;
using Quests;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Quest StartQuest;
        [SerializeField] private GameObject player;
        
        private Area currentArea;

        private void Start()
        {
            QuestManager.Instance.SetQuest(StartQuest);
            QuestManager.Instance.OnQuestComplete += QuestCompleted;
            var healthSystem = player.GetComponent<HealthSystem>();
            healthSystem.OnDeath += PlayerDied;
            healthSystem.OnDamageTaken += PlayerDamaged;
        }

        private void QuestCompleted(Quest quest)
        {
            QuestManager.Instance.SetQuest(quest.nextQuest);
        }

        // Wrapper
        private void PlayerDied()
        {
            Restart();
        }

        private void PlayerDamaged(Vector3 position, HumanTarget attacker)
        {
            if (currentArea.AreaTrigger.bounds.Contains(position) && !currentArea.IsActive)
            {
                currentArea.Activate();
            }
        }

        private void Restart()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}