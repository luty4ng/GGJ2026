using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    public interface IRoutableInputHandler
    {
        void OnRightClick(Vector2 pos);
        void OnLeftClick(Vector2 pos);
        void OnMousePosition(Vector2 pos);
        void OnCancel();
    }

    public sealed class InputRouter
    {
        public IRoutableInputHandler Handler { get; set; }

        public InputRouter(IRoutableInputHandler handler)
        {
            Handler = handler;
        }

        public void OnRightClick(Vector2 screenPos)
        {
            if (IsPointerOnUI(screenPos))
                return;
            Handler?.OnRightClick(screenPos);
        }

        public void OnLeftClick(Vector2 screenPos)
        {
            if (IsPointerOnUI(screenPos))
                return;
            Handler?.OnLeftClick(screenPos);
        }

        public void OnMousePosition(Vector2 screenPos)
        {
            if (IsPointerOnUI(screenPos))
                return;
            Handler?.OnMousePosition(screenPos);
        }

        public void OnCancel()
        {
            Handler?.OnCancel();
        }

        private static readonly PointerEventData _ped = new PointerEventData(EventSystem.current);
        private static readonly List<RaycastResult> _results = new List<RaycastResult>();
        public static bool IsPointerOnUI(Vector2 screenPos)
        {
            if (EventSystem.current == null) return false;

            _ped.position = screenPos;
            _results.Clear();
            EventSystem.current.RaycastAll(_ped, _results);

            return _results.Count > 0;
        }
    }
}
