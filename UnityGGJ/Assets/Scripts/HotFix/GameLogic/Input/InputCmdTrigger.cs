using UnityEngine;
using System.Collections.Generic;

namespace UnityGameFramework.Runtime
{
    public class InputCmdTrigger : MonoBehaviour
    {
        [System.Serializable]
        public struct InputBinding
        {
            public string eventKey;
            public string keyboardKey;
            public string gamepadBtn;
            public bool triggerOnPress;
            public bool triggerOnHold;
            public bool triggerOnRelease;
            public float holdThreshold;

            public InputBinding(string eventKey, string keyboardKey, string gamepadBtn, bool triggerOnPress,
            bool triggerOnHold, bool triggerOnRelease, float holdThreshold)
            {
                this.eventKey = eventKey;
                this.keyboardKey = keyboardKey;
                this.gamepadBtn = gamepadBtn;
                this.triggerOnPress = triggerOnPress;
                this.triggerOnHold = triggerOnHold;
                this.triggerOnRelease = triggerOnRelease;
                this.holdThreshold = holdThreshold;
            }
        }

        public List<InputBinding> inputBindings = new List<InputBinding>();
        private Dictionary<string, InputBinding> m_BindingMap;
        private Dictionary<string, float> m_HoldTimers;
        private Dictionary<string, bool> m_PressedStates;

        private void Awake()
        {
            InitializeInputSystem();
            SetupDefaultBindings();
        }

        private void InitializeInputSystem()
        {
            m_BindingMap = new Dictionary<string, InputBinding>();
            m_HoldTimers = new Dictionary<string, float>();
            m_PressedStates = new Dictionary<string, bool>();
        }

        private void SetupDefaultBindings()
        {
            // if (inputBindings.Count == 0)
            // {
            //     inputBindings = new List<InputBinding>
            //     {
            //         new InputBinding { eventKey = InputCmdKey.Jump, keyboardKey = "space", gamepadBtn = "buttonSouth", triggerOnPress = true },
            //         new InputBinding { eventKey = InputCmdKey.Sprint, keyboardKey = "left shift", gamepadBtn = "buttonEast", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.Crouch, keyboardKey = "left ctrl", gamepadBtn = "rightShoulder", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.Interact, keyboardKey = "e", gamepadBtn = "buttonWest", triggerOnPress = true },
            //         new InputBinding { eventKey = InputCmdKey.Pause, keyboardKey = "escape", gamepadBtn = "start", triggerOnPress = true },
            //         new InputBinding { eventKey = InputCmdKey.MoveForward, keyboardKey = "w", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.MoveBackward, keyboardKey = "s", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.MoveLeft, keyboardKey = "a", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.MoveRight, keyboardKey = "d", triggerOnHold = true },
            //         new InputBinding { eventKey = InputCmdKey.Reload, keyboardKey = "r", gamepadBtn = "buttonNorth", triggerOnPress = true }
            //     };
            // }

            foreach (var binding in inputBindings)
            {
                m_BindingMap[binding.eventKey] = binding;
                m_HoldTimers[binding.eventKey] = 0f;
                m_PressedStates[binding.eventKey] = false;
            }
        }

        private void Update()
        {
            foreach (var binding in inputBindings)
            {
                bool isPressed = false;

                if (!string.IsNullOrEmpty(binding.keyboardKey))
                    isPressed = Input.GetKey(binding.keyboardKey);
                HandleInputState(binding.eventKey, isPressed);
            }
        }

        private void HandleInputState(string eventKey, bool isPressed)
        {
            if (!m_BindingMap.TryGetValue(eventKey, out var binding))
                return;

            bool wasPressed = m_PressedStates[eventKey];
            m_PressedStates[eventKey] = isPressed;

            if (isPressed && !wasPressed && binding.triggerOnPress)
            {
                // InputCmdWrapper.Instance.TriggerCmd(eventKey);
            }
            else if (!isPressed && wasPressed && binding.triggerOnRelease)
            {
                // InputCmdWrapper.Instance.TriggerCmd(eventKey);
            }
            else if (isPressed && binding.triggerOnHold)
            {
                m_HoldTimers[eventKey] += Time.deltaTime;
                if (m_HoldTimers[eventKey] >= binding.holdThreshold)
                {
                    // InputCmdWrapper.Instance.TriggerCmd(eventKey);
                    m_HoldTimers[eventKey] = 0f;
                }
            }
            else if (!isPressed)
            {
                m_HoldTimers[eventKey] = 0f;
            }
        }

        public void AddBinding(string eventKey, string keyboardKey, string gamepadBtn = "",
            bool triggerOnPress = true, bool triggerOnHold = false, bool triggerOnRelease = false)
        {
            var newBinding = new InputBinding
            {
                eventKey = eventKey,
                keyboardKey = keyboardKey,
                gamepadBtn = gamepadBtn,
                triggerOnPress = triggerOnPress,
                triggerOnHold = triggerOnHold,
                triggerOnRelease = triggerOnRelease
            };

            inputBindings.Add(newBinding);
            m_BindingMap[eventKey] = newBinding;
            m_HoldTimers[eventKey] = 0f;
            m_PressedStates[eventKey] = false;
        }

        public void RemoveBinding(string eventKey)
        {
            inputBindings.RemoveAll(b => b.eventKey == eventKey);
            m_BindingMap.Remove(eventKey);
            m_HoldTimers.Remove(eventKey);
            m_PressedStates.Remove(eventKey);
        }

        public bool IsKeyPressed(string eventKey)
        {
            return m_PressedStates.TryGetValue(eventKey, out var pressed) && pressed;
        }

        public float GetKeyHoldTime(string eventKey)
        {
            return m_HoldTimers.TryGetValue(eventKey, out var time) ? time : 0f;
        }
    }
}