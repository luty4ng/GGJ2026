using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [ExecuteAlways]
    public class UIGridLayoutAdaptor : MonoBehaviour
    {
        [SerializeField] private bool m_adaptWidth = true;
        [SerializeField] private bool m_adaptHeight = true;

        private GridLayoutGroup m_gridLayoutGroup;
        private RectTransform m_rectTransform;
        private int m_lastChildCount = -1;
        private Vector2 m_lastCellSize;
        private Vector2 m_lastSpacing;
        private RectOffset m_lastPadding;

        private void OnEnable()
        {
            MakeSureRequiredComponents();
            UpdateLayout();
        }

        private void Update()
        {
            if (HasLayoutChanged())
            {
                UpdateLayout();
            }
        }

        private bool HasLayoutChanged()
        {
            int childCount = m_gridLayoutGroup.transform.childCount;
            bool changed = m_lastChildCount != childCount ||
                           m_lastCellSize != m_gridLayoutGroup.cellSize ||
                           m_lastSpacing != m_gridLayoutGroup.spacing ||
                           !IsPaddingEqual(m_lastPadding, m_gridLayoutGroup.padding);

            if (changed)
            {
                m_lastChildCount = childCount;
                m_lastCellSize = m_gridLayoutGroup.cellSize;
                m_lastSpacing = m_gridLayoutGroup.spacing;
                m_lastPadding = new RectOffset(
                    m_gridLayoutGroup.padding.left,
                    m_gridLayoutGroup.padding.right,
                    m_gridLayoutGroup.padding.top,
                    m_gridLayoutGroup.padding.bottom
                );
            }

            return changed;
        }

        private bool IsPaddingEqual(RectOffset a, RectOffset b)
        {
            if (a == null || b == null)
            {
                return a == b;
            }
            return a.left == b.left && a.right == b.right && a.top == b.top && a.bottom == b.bottom;
        }

        public void UpdateLayout()
        {
            if (m_gridLayoutGroup == null || m_rectTransform == null)
            {
                return;
            }

            int activeChildCount = GetActiveChildCount();
            if (activeChildCount == 0)
            {
                return;
            }

            Vector2 cellSize = m_gridLayoutGroup.cellSize;
            Vector2 spacing = m_gridLayoutGroup.spacing;
            RectOffset padding = m_gridLayoutGroup.padding;

            int columns = 1;
            int rows = 1;

            if (m_gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            {
                columns = m_gridLayoutGroup.constraintCount;
                rows = Mathf.CeilToInt((float)activeChildCount / columns);
            }
            else if (m_gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            {
                rows = m_gridLayoutGroup.constraintCount;
                columns = Mathf.CeilToInt((float)activeChildCount / rows);
            }
            else
            {
                float availableWidth = m_rectTransform.rect.width - padding.left - padding.right;
                columns = Mathf.Max(1, Mathf.FloorToInt((availableWidth + spacing.x) / (cellSize.x + spacing.x)));
                rows = Mathf.CeilToInt((float)activeChildCount / columns);
            }

            Vector2 newSize = m_rectTransform.sizeDelta;

            if (m_adaptWidth)
            {
                float width = padding.left + padding.right + 
                              (cellSize.x * columns) + 
                              (spacing.x * columns);
                newSize.x = width;
            }

            if (m_adaptHeight)
            {
                float height = padding.top + padding.bottom + 
                               (cellSize.y * rows) + 
                               (spacing.y * 2 * rows);
                newSize.y = height;
            }

            m_rectTransform.sizeDelta = newSize;
        }

        private int GetActiveChildCount()
        {
            int count = 0;
            for (int i = 0; i < m_gridLayoutGroup.transform.childCount; i++)
            {
                if (m_gridLayoutGroup.transform.GetChild(i).gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }

        public void ForceUpdate()
        {
            m_lastChildCount = -1;
            UpdateLayout();
        }

        private void MakeSureRequiredComponents()
        {
            if (m_gridLayoutGroup == null)
                m_gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            MakeSureRequiredComponents();
            UpdateLayout();
        }
#endif
    }
}

