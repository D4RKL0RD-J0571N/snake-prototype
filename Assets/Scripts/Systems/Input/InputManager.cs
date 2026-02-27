using UnityEngine;
using UnityEngine.InputSystem;
using SnakePrototype.Core;
using SnakePrototype.Events;

namespace SnakePrototype.Systems.Input
{
    public class InputManager : IGameService
    {
        private InputActionAsset _inputAsset;
        private InputAction _moveAction;
        private InputAction _pauseAction;

        [Header("Input Settings")]
        [SerializeField] private float _deadzone = 0.1f;
        [SerializeField] private float _sensitivity = 1.0f;
        [SerializeField] private bool _useRawInput = false;

        public void Initialize()
        {
            Debug.Log("InputManager Initializing...");
            
            // Load the asset (assuming it's in a Resources folder or we construct it manually)
            // For a prototype without Addressables/Resources setup, manually defining actions is fastest/safest.
            // But let's try to load the asset file if possible, or fallback to code definition.
            
            // 1. Move Action (Stick + DPad + WASD + Arrows)
            _moveAction = new InputAction("Move", binding: "<Gamepad>/leftStick");
            _moveAction.AddBinding("<Gamepad>/dpad"); // Direct DPad support
            
            _moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            // 2. Pause Action
            _pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
            _pauseAction.AddBinding("<Keyboard>/p");
            _pauseAction.AddBinding("<Gamepad>/start");

            // 3. Confirm/Cancel (For UI and Flow)
            var confirmAction = new InputAction("Confirm", binding: "<Keyboard>/enter");
            confirmAction.AddBinding("<Keyboard>/space");
            confirmAction.AddBinding("<Gamepad>/buttonSouth"); // A

            var cancelAction = new InputAction("Cancel", binding: "<Keyboard>/backspace");
            cancelAction.AddBinding("<Gamepad>/buttonEast"); // B

            _moveAction.performed += OnMove;
            _pauseAction.performed += OnPause;
            confirmAction.performed += ctx => GameEventManager.Publish(new RespawnEvent()); // Temporary hook for confirm
            
            _moveAction.Enable();
            _pauseAction.Enable();
            confirmAction.Enable();
            cancelAction.Enable();

            Debug.Log($"InputManager Initialized (Sensitivity: {_sensitivity}, Deadzone: {_deadzone})");
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            
            // Apply deadzone
            if (input.magnitude < _deadzone)
            {
                input = Vector2.zero;
            }
            else
            {
                // Apply sensitivity
                if (!_useRawInput)
                {
                    input = input * _sensitivity;
                    
                    // Clamp to prevent overshooting
                    input = Vector2.ClampMagnitude(input, 1.0f);
                }
            }
            
            // Only publish if non-zero, or maybe we want to publish zero to stop? 
            // Snake usually doesn't stop.
            if (input != Vector2.zero)
            {
                GameEventManager.Publish(new MoveInputEvent(input));
            }
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            // Toggle pause state logic usually resides in Game/Time manager, but input is event-driven
            // GameEventManager.Publish(new PauseInputEvent()); 
            // (PauseInputEvent is defined in GameEvents.cs)
        }

        public void Tick() 
        { 
            // improved: no polling needed!
        }

        public void Shutdown()
        {
            _moveAction.Disable();
            _pauseAction.Disable();
            
            _moveAction.performed -= OnMove;
            _pauseAction.performed -= OnPause;
            
            _moveAction.Dispose();
            _pauseAction.Dispose();
        }
    }
}
