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
        private InputAction _confirmAction;
        private InputAction _cancelAction;

        private GameState _currentGameState = GameState.Playing;

        private float _deadzone = 0.1f;
        private float _sensitivity = 1.0f;
        private bool _useRawInput = false;

        public void Initialize()
        {
            Debug.Log("InputManager Initializing...");
            
            GameEventManager.AddListener<GameStateChangedEvent>(OnGameStateChanged);
            
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
            _confirmAction = new InputAction("Confirm", binding: "<Keyboard>/enter");
            _confirmAction.AddBinding("<Keyboard>/space");
            _confirmAction.AddBinding("<Gamepad>/buttonSouth"); // A

            _cancelAction = new InputAction("Cancel", binding: "<Keyboard>/backspace");
            _cancelAction.AddBinding("<Gamepad>/buttonEast"); // B

            _moveAction.performed += OnMove;
            _pauseAction.performed += OnPause;
            _confirmAction.performed += OnConfirm;
            
            _moveAction.Enable();
            _pauseAction.Enable();
            _confirmAction.Enable();
            _cancelAction.Enable();

            Debug.Log($"InputManager Initialized (Sensitivity: {_sensitivity}, Deadzone: {_deadzone})");
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _currentGameState = e.NewState;
        }

        private void OnConfirm(InputAction.CallbackContext context)
        {
            // Gate RespawnEvent behind GameOver
            if (_currentGameState == GameState.GameOver)
            {
                GameEventManager.Publish(new RespawnEvent());
            }

            // Always publish generic confirm for other systems
            GameEventManager.Publish(new ConfirmEvent());
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
