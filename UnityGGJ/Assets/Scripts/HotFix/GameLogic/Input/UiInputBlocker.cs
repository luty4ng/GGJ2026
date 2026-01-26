#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// UI 交互时的输入阻断器：用于屏蔽游戏输入（相机/角色/能力等）误触发。
    /// </summary>
    public static class UiInputBlocker
    {
        private static int s_manualBlockCount;
        private static EventSystem? s_cachedEventSystem;
        private static PointerEventData? s_pointerEventData;
        private static readonly List<RaycastResult> s_results = new List<RaycastResult>(16);
        private static readonly HashSet<string> s_nonBlockingUiLayerNames = new HashSet<string>
        {
            "WorldUI"
            // 在这里添加更多不需要阻断输入的层名称
            // 例如: "HUD", "Tooltip", "FloatingUI"
        };

        public static bool IsBlocked(string groupName)
        {
            if (s_manualBlockCount > 0)
            {
                return true;
            }

            if (!IsPointerOnUi())
            {
                return false;
            }

            return groupName == InputCmdKey.Group.Topdown
                   || groupName == InputCmdKey.Group.Constructing
                   || groupName == InputCmdKey.Group.AbilityCast;
        }

        public static void PushBlock()
        {
            s_manualBlockCount++;
        }

        public static void PopBlock()
        {
            if (s_manualBlockCount <= 0)
            {
                s_manualBlockCount = 0;
                return;
            }

            s_manualBlockCount--;
        }

        private static bool IsPointerOnUi()
        {
            var es = EventSystem.current;
            if (es == null)
            {
                return false;
            }

            if (Mouse.current != null)
            {
                return RaycastUi(es, Mouse.current.position.ReadValue());
            }

            if (Touchscreen.current != null)
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (!touch.press.isPressed)
                    {
                        continue;
                    }

                    if (RaycastUi(es, touch.position.ReadValue()))
                    {
                        return true;
                    }
                }
            }

            return es.IsPointerOverGameObject();
        }

        private static bool RaycastUi(EventSystem eventSystem, Vector2 screenPos)
        {
            if (s_pointerEventData == null || s_cachedEventSystem != eventSystem)
            {
                s_cachedEventSystem = eventSystem;
                s_pointerEventData = new PointerEventData(eventSystem);
            }

            s_pointerEventData.position = screenPos;
            s_results.Clear();
            eventSystem.RaycastAll(s_pointerEventData, s_results);

            if (s_results.Count == 0)
            {
                return false;
            }

            // 获取所有非阻断层的索引
            var nonBlockingLayers = new HashSet<int>();
            foreach (var layerName in s_nonBlockingUiLayerNames)
            {
                int layerIndex = LayerMask.NameToLayer(layerName);
                if (layerIndex != -1)
                {
                    nonBlockingLayers.Add(layerIndex);
                }
            }

            // 如果没有定义任何非阻断层，则所有 UI 都阻断输入
            if (nonBlockingLayers.Count == 0)
            {
                return true;
            }

            // 检查是否有任何 UI 元素不在非阻断层中
            foreach (var result in s_results)
            {
                if (result.gameObject != null)
                {
                    // 如果对象不在任何非阻断层中，则阻断输入
                    if (!nonBlockingLayers.Contains(result.gameObject.layer))
                    {
                        return true;
                    }
                }
            }

            // 所有 UI 元素都在非阻断层中，不阻断输入
            return false;
        }
    }
}


