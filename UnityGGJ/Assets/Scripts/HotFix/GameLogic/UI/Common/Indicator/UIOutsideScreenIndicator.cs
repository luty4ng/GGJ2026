using UnityEngine;

namespace GameLogic
{
    public class UIOutsideScreenIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectEdge;
        private Canvas m_canvas;
        private RectTransform m_rectTransform;
        private RectTransform m_parentRectTransform;
        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_canvas = m_rectTransform.GetComponentInParent<Canvas>();
            m_parentRectTransform = m_rectTransform.parent as RectTransform;
        }

        public void UpdatePosAndRot(Transform target, Camera cam, Vector3 targetWorldPos, float edgePadding, out bool visible)
        {
            visible = true;
            // 世界坐标 -> 屏幕坐标（像素）。z<0 表示目标在相机背后
            Vector3 screenPos = cam.WorldToScreenPoint(targetWorldPos);

            if (screenPos.z < 0f)
            {
                // 背后目标：将屏幕点镜像到前方，保证“方向”计算仍然可用
                screenPos.x = Screen.width - screenPos.x;
                screenPos.y = Screen.height - screenPos.y;
                screenPos.z = 0f;
            }

            // 目标在屏幕内：隐藏指示器
            bool isOnScreen = screenPos.z > 0f
                              && screenPos.x >= 0f
                              && screenPos.x <= Screen.width
                              && screenPos.y >= 0f
                              && screenPos.y <= Screen.height;

            if (isOnScreen)
            {
                visible = false;
                return;
            }

            // ScreenSpaceOverlay 不需要 UI 相机；其它模式需要传入 Canvas.worldCamera 进行正确换算
            Camera uiCamera = null;
            if (m_canvas != null && m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCamera = m_canvas.worldCamera != null ? m_canvas.worldCamera : cam;
            }

            // 屏幕坐标 -> 父 RectTransform 的本地坐标（注意：这是父Rect的本地坐标系，不等同于 anchoredPosition）
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_parentRectTransform, screenPos, uiCamera, out Vector2 localTargetPos))
            {
                visible = false;
                return;
            }

            Rect parentRect = m_parentRectTransform.rect;
            Vector2 center = parentRect.center;

            // 计算可放置区域：父Rect半宽高减去自身半尺寸 + padding，确保指示器不会“出屏”
            Vector2 widgetHalfSize = m_rectTransform.rect.size * 0.5f;
            float extentX = parentRect.width * 0.5f - (widgetHalfSize.x + edgePadding);
            float extentY = parentRect.height * 0.5f - (widgetHalfSize.y + edgePadding);
            extentX = Mathf.Max(0f, extentX);
            extentY = Mathf.Max(0f, extentY);

            // 从父Rect中心指向目标的方向
            Vector2 dirFromCenter = localTargetPos - center;
            Vector2 localEdgePos = center;
            if (dirFromCenter.sqrMagnitude > 0.0001f)
            {
                // 计算“中心->目标”的射线与矩形边界的交点：取 X/Y 方向上最先碰到边界的缩放比例
                float absX = Mathf.Abs(dirFromCenter.x);
                float absY = Mathf.Abs(dirFromCenter.y);

                float scaleX = absX > 0.0001f ? extentX / absX : float.PositiveInfinity;
                float scaleY = absY > 0.0001f ? extentY / absY : float.PositiveInfinity;
                float scale = Mathf.Min(scaleX, scaleY);

                localEdgePos = center + dirFromCenter * scale;
            }

            // localEdgePos 是“父本地坐标”，需要转换成 child 的 anchoredPosition（适配任意锚点）
            Vector2 anchoredEdgePos = LocalToAnchoredPosition(m_parentRectTransform, m_rectTransform, localEdgePos);
            m_rectTransform.anchoredPosition = anchoredEdgePos;

            // 箭头方向：优先用“贴边点 -> 目标点”，这样指向更直观；退化到“中心->目标”
            Vector2 dirForArrow = localTargetPos - localEdgePos;
            if (dirForArrow.sqrMagnitude <= 0.0001f)
            {
                dirForArrow = dirFromCenter;
            }

            if (dirForArrow.sqrMagnitude > 0.0001f && m_rectEdge != null)
            {
                // 角度基准：0° 为 +X（向右），逆时针为正；若你的箭头贴图默认朝上，可在这里做额外角度偏移
                float angle = Mathf.Atan2(dirForArrow.y, dirForArrow.x) * Mathf.Rad2Deg;
                m_rectEdge.localEulerAngles = new Vector3(0f, 0f, angle);
            }
        }

        private static Vector2 LocalToAnchoredPosition(RectTransform parent, RectTransform child, Vector2 parentLocalPos)
        {
            // 将“父Rect本地坐标”转换为“子物体 anchoredPosition”
            // anchoredPosition 的参考点是 anchorMin/anchorMax 的中心位置对应的父本地坐标
            Rect parentRect = parent.rect;
            Vector2 anchorCenter = (child.anchorMin + child.anchorMax) * 0.5f;
            Vector2 anchorRefLocalPos = new Vector2(
                Mathf.Lerp(parentRect.xMin, parentRect.xMax, anchorCenter.x),
                Mathf.Lerp(parentRect.yMin, parentRect.yMax, anchorCenter.y)
            );
            return parentLocalPos - anchorRefLocalPos;
        }

    }
}
