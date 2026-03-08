using Areas;
using Core;
using FPSDemo.Core;
using FPSDemo.Input;
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
        [SerializeField] private InputManager inputManager; 
        
        private Area currentArea;
        private HealthSystem playerHealthSystem;

        private void Start()
        {
            DialogueManager.Instance.DialogFinished += DialogFinished;
            
            QuestManager.Instance.SetQuest(StartQuest);
            QuestManager.Instance.OnQuestComplete += QuestCompleted;
            
            playerHealthSystem = player.GetComponent<HealthSystem>();
            if (playerHealthSystem != null)
            {
                playerHealthSystem.OnDeath += PlayerDied;
                playerHealthSystem.OnDamageTaken += PlayerDamaged;
            }
        }

        protected void OnDestroy()
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.DialogFinished -= DialogFinished;
            
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnQuestComplete -= QuestCompleted;
                
            if (playerHealthSystem != null)
            {
                playerHealthSystem.OnDeath -= PlayerDied;
                playerHealthSystem.OnDamageTaken -= PlayerDamaged;
            }
        }
        
        private void Update()
        {
            CheckLocationQuests();
        }

        private void CheckLocationQuests()
        {
            var activeQuest = QuestManager.Instance.ActiveQuest;
            if (activeQuest == null) return;

            foreach (var subGoal in activeQuest.subGoals)
            {
                if (subGoal.completionType != CompletionType.Automatic || !subGoal.useRadius) 
                    continue;

                Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z); // Uprav podľa toho, či robíš 2D alebo 3D hru
        
                if (Vector2.Distance(playerPos2D, subGoal.goalPosition) <= subGoal.radius)
                {
                    QuestManager.Instance.CompleteSubGoal(subGoal);
                    break;
                }
            }
        }

        private void QuestCompleted(Quest quest)
        {
            
        }

        private void DialogFinished()
        {
            inputManager.SetActiveAllGameplayControls(true);
            Game.ToggleCursor(false);
            
            var activeQuest = QuestManager.Instance.ActiveQuest;
            if (QuestManager.TryGetSubGoalByCompletion(activeQuest, CompletionType.Dialog, out var subGoal))
            {
                if (subGoal.subName == "Promluv si s bláznem")
                    QuestManager.Instance.CompleteSubGoal(subGoal);
            }
        }

        private void PlayerDied()
        {
            Restart();
        }

        private void PlayerDamaged(Vector3 position, HumanTarget attacker)
        {
            if (currentArea != null && !currentArea.IsActive && currentArea.AreaTrigger != null)
            {
                if (currentArea.AreaTrigger.bounds.Contains(position))
                {
                    currentArea.Activate();
                }
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

        public void ClearArea(Area area)
        {
            if (currentArea == area)
            {
                currentArea = null;
            }
        }
        
        public void CompleteCurrentArea()
        {
            if (currentArea != null && currentArea.IsActive)
            {
                currentArea.Deactivate();
                currentArea = null;
            }
        }
    }
}