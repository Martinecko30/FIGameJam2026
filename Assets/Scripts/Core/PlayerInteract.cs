using Dialogue;
using FPSDemo.Core;
using FPSDemo.Input;
using FPSDemo.NPC;
using Managers;
using Quests;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private InputAction interactAction;
        
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputManager _inputManager;
        
        [SerializeField] private PoisonableItem poisonableItem;
        [Header("Goals")] [SerializeField] private SubGoal finishPoison;

        private bool hasPoison = false;
        
        private void Awake()
        {
            interactAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/f");

            interactAction.performed += context => PerformAction();
        }

        private void PerformAction()
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 100f))
            {
                if (hit.transform.gameObject.TryGetComponent<Dialog>(out var dialog))
                {
                    _inputManager.SetActiveAllGameplayControls(false);
                    Game.ToggleCursor(true);
                    
                    dialog.ContinueDialogue();
                }
                
                else if (!hasPoison && hit.transform.name == "poison")
                {
                    hasPoison = true;
                    hit.transform.gameObject.SetActive(false);
                }
                
                else if (hasPoison && hit.transform.name == "Golden Goblet")
                {
                    poisonableItem.IsPoisoned = true;
                    QuestManager.Instance.CompleteSubGoal(finishPoison);
                }
            }
        }
        
        private void OnEnable()
        {
            interactAction.Enable();
        }

        private void OnDisable()
        {
            interactAction.Disable();
        }
    }
}