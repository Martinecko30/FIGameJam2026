using Dialogue;
using FPSDemo.Core;
using FPSDemo.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private InputAction interactAction;
        
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputManager _inputManager;
        
        
        
        private void Awake()
        {
            interactAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/f");

            interactAction.performed += context => PerformAction();
        }

        private void PerformAction()
        {
            if (Physics.Raycast(transform.position, transform.forward, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<Dialog>(out var dialog))
                {
                    _inputManager.SetActiveAllGameplayControls(false);
                    Game.ToggleCursor(true);
                    
                    dialog.ContinueDialogue();
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