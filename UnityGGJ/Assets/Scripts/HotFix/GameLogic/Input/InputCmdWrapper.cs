using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using GameLogic;
using System;
using GameFramework.WebRequest;

namespace UnityGameFramework.Runtime
{
    public interface ICmdInfo
    {
        Action<InputAction.CallbackContext> OnPerformed { get; }
        Action<InputAction.CallbackContext> OnStarted { get; }
        Action<InputAction.CallbackContext> OnCanceled { get; }

        Delegate RawPerform { get; }
        Delegate RawStarted { get; }
        Delegate RawCanceled { get; }
    }
    public class CmdInfo : ICmdInfo
    {
        public Action<InputAction.CallbackContext> OnPerformed { get; set; }
        public Action<InputAction.CallbackContext> OnStarted { get; set; }
        public Action<InputAction.CallbackContext> OnCanceled { get; set; }
        public Delegate RawPerform { get; private set; }
        public Delegate RawStarted { get; private set; }
        public Delegate RawCanceled { get; private set; }
        private readonly string m_groupName;

        public CmdInfo(string groupName, Action<InputAction.CallbackContext> onPerformed, Action<InputAction.CallbackContext> onStarted, Action<InputAction.CallbackContext> onCanceled)
        {
            m_groupName = groupName;
            OnPerformed = onPerformed;
            OnStarted = onStarted;
            OnCanceled = onCanceled;
            RawPerform = onPerformed;
            RawStarted = onStarted;
            RawCanceled = onCanceled;
        }

        public CmdInfo(string groupName, Action onPerformed, Action onStarted, Action onCanceled)
        {
            m_groupName = groupName;
            RawPerform = onPerformed;
            RawStarted = onStarted;
            RawCanceled = onCanceled;

            OnPerformed = ctx =>
            {
                if (UiInputBlocker.IsBlocked(m_groupName))
                {
                    return;
                }

                onPerformed?.Invoke();
            };
            OnStarted = ctx =>
            {
                if (UiInputBlocker.IsBlocked(m_groupName))
                {
                    return;
                }

                onStarted?.Invoke();
            };
            OnCanceled = ctx => onCanceled?.Invoke();
        }
    }

    public class CmdInfo<T> : ICmdInfo where T : struct
    {
        public Action<InputAction.CallbackContext> OnPerformed { get; set; }
        public Action<InputAction.CallbackContext> OnStarted { get; set; }
        public Action<InputAction.CallbackContext> OnCanceled { get; set; }
        public Delegate RawPerform { get; private set; }
        public Delegate RawStarted { get; private set; }
        public Delegate RawCanceled { get; private set; }
        private readonly string m_groupName;
        private readonly IInputSmoother<T> m_smoother;

        public CmdInfo(string groupName, Action<T> onPerformed, Action<T> onStarted, Action<T> onCanceled,
                       IInputSmoother<T> smoother = null)
        {
            m_groupName = groupName;
            m_smoother = smoother;
            RawPerform = onPerformed;
            RawStarted = onStarted;
            RawCanceled = onCanceled;
            OnPerformed = ctx =>
            {
                if (UiInputBlocker.IsBlocked(m_groupName))
                {
                    return;
                }

                onPerformed?.Invoke(GetSmoothedValue(ctx));
            };
            OnStarted = ctx =>
            {
                if (UiInputBlocker.IsBlocked(m_groupName))
                {
                    return;
                }

                onStarted?.Invoke(GetSmoothedValue(ctx));
            };
            OnCanceled = ctx => onCanceled?.Invoke(GetSmoothedValue(ctx));
        }

        private T GetSmoothedValue(InputAction.CallbackContext ctx)
        {
            var rawValue = ctx.ReadValue<T>();
            if (m_smoother != null)
                return m_smoother.GetSmoothedValue(rawValue, Time.unscaledDeltaTime);
            return rawValue;
        }
    }

    public class InputCmdWrapper : Singleton<InputCmdWrapper>
    {
        private Dictionary<string, Dictionary<string, List<ICmdInfo>>> m_CmdGroups;
        private InputActionAsset m_InputActionAsset;
        private Dictionary<string, InputAction> m_InputActions;
        private Dictionary<string, List<InputAction>> m_InputGroupToActions;
        public InputCmdWrapper()
        {
            m_CmdGroups = new Dictionary<string, Dictionary<string, List<ICmdInfo>>>();
            m_InputActions = new Dictionary<string, InputAction>();
            m_InputGroupToActions =  new Dictionary<string, List<InputAction>>();
            InitializeInputMaps();
        }

        private void InitializeInputMaps()
        {
            var gameplayInputMaps = new GameplayInputMaps();
            m_InputActionAsset = gameplayInputMaps.asset;
            m_InputActionAsset.Enable();
            RegisterInputActions();
        }

        private void RegisterInputActions()
        {
            foreach (var actionMap in m_InputActionAsset.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    string actionKey = $"{actionMap.name}.{action.name}";
                    m_InputActions[actionKey] = action;
                    if(!m_InputGroupToActions.ContainsKey(actionMap.name))
                        m_InputGroupToActions[actionMap.name] = new List<InputAction>();
                    m_InputGroupToActions[actionMap.name].Add(action);
                }
            }
        }

        public void ListenCmd(string groupName, string actionName, Action onPerform, Action onStarted = null, Action onCanceled = null)
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            var cmdInfo = new CmdInfo(groupName, onPerform, onStarted, onCanceled);
            m_InputActions[actionKey].performed += cmdInfo.OnPerformed;
            m_InputActions[actionKey].started += cmdInfo.OnStarted;
            m_InputActions[actionKey].canceled += cmdInfo.OnCanceled;
            CacheCmdInfo(groupName, actionName, cmdInfo);
        }

        public void ListenCmd(string groupName, string actionName, Action<InputAction.CallbackContext> onPerform, Action<InputAction.CallbackContext> onStarted = null, Action<InputAction.CallbackContext> onCanceled = null)
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            var cmdInfo = new CmdInfo(groupName,
                ctx =>
                {
                    if (UiInputBlocker.IsBlocked(groupName))
                    {
                        return;
                    }

                    onPerform?.Invoke(ctx);
                },
                ctx =>
                {
                    if (UiInputBlocker.IsBlocked(groupName))
                    {
                        return;
                    }

                    onStarted?.Invoke(ctx);
                },
                ctx => onCanceled?.Invoke(ctx));
            m_InputActions[actionKey].performed += cmdInfo.OnPerformed;
            m_InputActions[actionKey].started += cmdInfo.OnStarted;
            m_InputActions[actionKey].canceled += cmdInfo.OnCanceled;
            CacheCmdInfo(groupName, actionName, cmdInfo);
        }

        public void ListenCmd<T>(string groupName, string actionName, Action<T> onPerform, Action<T> onStarted = null, Action<T> onCanceled = null) where T : struct
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            var cmdInfo = new CmdInfo<T>(groupName, onPerform, onStarted, onCanceled);
            m_InputActions[actionKey].performed += cmdInfo.OnPerformed;
            m_InputActions[actionKey].started += cmdInfo.OnStarted;
            m_InputActions[actionKey].canceled += cmdInfo.OnCanceled;
            CacheCmdInfo(groupName, actionName, cmdInfo);
        }

        public void UnListenCmd(string groupName, string actionName, Action onPerform, Action onStarted = null, Action onCanceled = null)
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            ClearCmdInfo(groupName, actionName, actionKey, onPerform, onStarted, onCanceled);
        }

        public void UnListenCmd(string groupName, string actionName, Action<InputAction.CallbackContext> onPerform, Action<InputAction.CallbackContext> onStarted = null, Action<InputAction.CallbackContext> onCanceled = null)
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            ClearCmdInfo(groupName, actionName, actionKey, onPerform, onStarted, onCanceled);
        }

        public void UnListenCmd<T>(string groupName, string actionName, Action<T> onPerform, Action<T> onStarted = null, Action<T> onCanceled = null) where T : struct
        {
            if (!IsCmdValid(groupName, actionName, out string actionKey))
                return;

            ClearCmdInfo(groupName, actionName, actionKey, onPerform, onStarted, onCanceled);
        }

        public void EnableActionMap(string groupName)
        {
            if (!m_InputGroupToActions.TryGetValue(groupName, out var actions))
                return;
            actions.ForEach(a => a.Enable());
        }

        public void DisableActionMap(string groupName)
        {
            if (!m_InputGroupToActions.TryGetValue(groupName, out var actions))
                return;
            actions.ForEach(a => a.Disable());
        }

        private bool IsCmdValid(string groupName, string actionName, out string actionKey)
        {
            actionKey = string.Empty;
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(actionName))
            {
                Debug.LogError("Invalid parameters for UnListenInputAction.");
                return false;
            }

            actionKey = $"{groupName}.{actionName}";
            if (!m_InputActions.ContainsKey(actionKey))
            {
                Debug.LogError($"InputAction not found: {actionKey}");
                return false;
            }
            return true;
        }

        private void CacheCmdInfo(string groupName, string cmdKey, ICmdInfo cmdInfo)
        {
            if (!m_CmdGroups.TryGetValue(groupName, out var group))
            {
                group = new Dictionary<string, List<ICmdInfo>>();
                m_CmdGroups[groupName] = group;
            }

            if (!group.TryGetValue(cmdKey, out var delegates))
            {
                delegates = new List<ICmdInfo>();
                group[cmdKey] = delegates;
            }

            if (!delegates.Contains(cmdInfo))
            {
                delegates.Add(cmdInfo);
            }
        }

        private void ClearCmdInfo(string groupName, string cmdKey, string actionKey, Delegate onPerform, Delegate onStarted, Delegate onCanceled)
        {
            if (!m_CmdGroups.TryGetValue(groupName, out var group))
                return;
            if (!group.TryGetValue(cmdKey, out var delegates))
                return;
            if (!m_InputActions.TryGetValue(actionKey, out var action))
                return;

            bool Matches(ICmdInfo info)
            {
                if (onPerform != null && !DelegateEquals(info.RawPerform, onPerform))
                    return false;
                if (onStarted != null && !DelegateEquals(info.RawStarted, onStarted))
                    return false;
                if (onCanceled != null && !DelegateEquals(info.RawCanceled, onCanceled))
                    return false;
                return true;
            }

            for (int i = delegates.Count - 1; i >= 0; i--)
            {
                var info = delegates[i];
                if (!Matches(info))
                    continue;

                action.performed -= info.OnPerformed;
                action.started -= info.OnStarted;
                action.canceled -= info.OnCanceled;
                delegates.RemoveAt(i);
            }

            if (delegates.Count == 0)
            {
                group.Remove(cmdKey);
                if (group.Count == 0)
                    m_CmdGroups.Remove(groupName);
            }
        }

        private bool DelegateEquals(Delegate a, Delegate b)
        {
            if (a == null || b == null)
                return a == b;
            return a.Equals(b);
        }

        public InputAction GetInputAction(string groupName, string actionName)
        {
            string actionKey = $"{groupName}.{actionName}";
            return m_InputActions.TryGetValue(actionKey, out var action) ? action : null;
        }
    }
}