using Areas;
using Core;
using FPSDemo.Core;
using FPSDemo.Input;
using FPSDemo.Target;
using Quests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private Quest StartQuest;
        [SerializeField] private GameObject player;
        
        [SerializeField] private InputManager inputManager; 
        
        private Area currentArea;

        private void Start()
        {
            DialogueManager.Instance.DialogFinished += delegate
            {
                inputManager.SetActiveAllGameplayControls(true);
                Game.ToggleCursor(false);
            };
            
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

        public void SetArea(Area area)
        {
            currentArea = area;
        }
    }
}