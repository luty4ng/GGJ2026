using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace GameLogic.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    [ExecuteAlways]
    public class UIGridAutoFill : MonoBehaviour
    {
        public enum FillMode
        {
            FixedColumns,
            FixedRows,
            FixedRowsAndColumns
        }
        
        public enum AspectMode
        {
            Square,
            Custom
        }
        
        [OnValueChanged(nameof(UpdateCellSize))]
        [SerializeField] private FillMode m_fillMode = FillMode.FixedColumns;
        
        [ShowIf(nameof(ShowColumns))]
        [MinValue(1)]
        [OnValueChanged(nameof(UpdateCellSize))]
        [SerializeField] private int m_columns = 3;
        
        [ShowIf(nameof(ShowRows))]
        [MinValue(1)]
        [OnValueChanged(nameof(UpdateCellSize))]
        [SerializeField] private int m_rows = 2;
        
        [ShowIf(nameof(ShowAspectSettings))]
        [OnValueChanged(nameof(UpdateCellSize))]
        [SerializeField] private AspectMode m_aspectMode = AspectMode.Square;
        
        [ShowIf(nameof(ShowAspectRatio))]
        [OnValueChanged(nameof(UpdateCellSize))]
        [SerializeField] private Vector2 m_aspectRatio = new Vector2(1, 1);
        
        private GridLayoutGroup m_gridLayoutGroup;
        private RectTransform m_rectTransform;
        private Vector2 m_lastSize;
        private FillMode m_lastMode;
        private int m_lastColumns;
        private int m_lastRows;
        
        private bool ShowColumns => m_fillMode == FillMode.FixedColumns || m_fillMode == FillMode.FixedRowsAndColumns;
        private bool ShowRows => m_fillMode == FillMode.FixedRows || m_fillMode == FillMode.FixedRowsAndColumns;
        private bool ShowAspectSettings => m_fillMode != FillMode.FixedRowsAndColumns;
        private bool ShowAspectRatio => ShowAspectSettings && m_aspectMode == AspectMode.Custom;
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnEnable()
        {
            UpdateCellSize();
        }
        
        private void Update()
        {
            if (HasChanged())
            {
                UpdateCellSize();
            }
        }
        
        private void OnRectTransformDimensionsChange()
        {
            UpdateCellSize();
        }
        
        private void Initialize()
        {
            if (m_gridLayoutGroup == null)
                m_gridLayoutGroup = GetComponent<GridLayoutGroup>();
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();
        }
        
        private bool HasChanged()
        {
            if (m_rectTransform == null)
                return false;
            
            Vector2 currentSize = m_rectTransform.rect.size;
            bool changed = m_lastSize != currentSize 
                || m_lastMode != m_fillMode
                || m_lastColumns != m_columns 
                || m_lastRows != m_rows;
            
            if (changed)
            {
                m_lastSize = currentSize;
                m_lastMode = m_fillMode;
                m_lastColumns = m_columns;
                m_lastRows = m_rows;
            }
            
            return changed;
        }
        
        public void UpdateCellSize()
        {
            Initialize();
            
            if (m_gridLayoutGroup == null || m_rectTransform == null)
                return;
            
            float width = m_rectTransform.rect.width;
            float height = m_rectTransform.rect.height;
            
            RectOffset padding = m_gridLayoutGroup.padding;
            Vector2 spacing = m_gridLayoutGroup.spacing;
            
            Vector2 cellSize = Vector2.zero;
            
            switch (m_fillMode)
            {
                case FillMode.FixedColumns:
                    cellSize = CalculateFixedColumns(width, height, padding, spacing);
                    m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    m_gridLayoutGroup.constraintCount = m_columns;
                    break;
                    
                case FillMode.FixedRows:
                    cellSize = CalculateFixedRows(width, height, padding, spacing);
                    m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                    m_gridLayoutGroup.constraintCount = m_rows;
                    break;
                    
                case FillMode.FixedRowsAndColumns:
                    cellSize = CalculateFixedRowsAndColumns(width, height, padding, spacing);
                    m_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    m_gridLayoutGroup.constraintCount = m_columns;
                    break;
            }
            
            m_gridLayoutGroup.cellSize = cellSize;
        }
        
        private Vector2 CalculateFixedColumns(float width, float height, RectOffset padding, Vector2 spacing)
        {
            float availableWidth = width - padding.left - padding.right - (spacing.x * (m_columns - 1));
            float cellWidth = availableWidth / m_columns;
            
            float cellHeight;
            if (m_aspectMode == AspectMode.Square)
            {
                cellHeight = cellWidth;
            }
            else
            {
                cellHeight = cellWidth / m_aspectRatio.x * m_aspectRatio.y;
            }
            
            return new Vector2(cellWidth, cellHeight);
        }
        
        private Vector2 CalculateFixedRows(float width, float height, RectOffset padding, Vector2 spacing)
        {
            float availableHeight = height - padding.top - padding.bottom - (spacing.y * (m_rows - 1));
            float cellHeight = availableHeight / m_rows;
            
            float cellWidth;
            if (m_aspectMode == AspectMode.Square)
            {
                cellWidth = cellHeight;
            }
            else
            {
                cellWidth = cellHeight / m_aspectRatio.y * m_aspectRatio.x;
            }
            
            return new Vector2(cellWidth, cellHeight);
        }
        
        private Vector2 CalculateFixedRowsAndColumns(float width, float height, RectOffset padding, Vector2 spacing)
        {
            float availableWidth = width - padding.left - padding.right - (spacing.x * (m_columns - 1));
            float cellWidth = availableWidth / m_columns;
            
            float availableHeight = height - padding.top - padding.bottom - (spacing.y * (m_rows - 1));
            float cellHeight = availableHeight / m_rows;
            
            return new Vector2(cellWidth, cellHeight);
        }
        
        public void SetFillMode(FillMode mode)
        {
            m_fillMode = mode;
            UpdateCellSize();
        }
        
        public void SetColumns(int columns)
        {
            m_columns = Mathf.Max(1, columns);
            UpdateCellSize();
        }
        
        public void SetRows(int rows)
        {
            m_rows = Mathf.Max(1, rows);
            UpdateCellSize();
        }
        
        public void SetAspectMode(AspectMode mode)
        {
            m_aspectMode = mode;
            UpdateCellSize();
        }
        
        public void SetAspectRatio(Vector2 ratio)
        {
            m_aspectRatio = ratio;
            UpdateCellSize();
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            m_columns = Mathf.Max(1, m_columns);
            m_rows = Mathf.Max(1, m_rows);
            m_aspectRatio.x = Mathf.Max(0.1f, m_aspectRatio.x);
            m_aspectRatio.y = Mathf.Max(0.1f, m_aspectRatio.y);
            
            Initialize();
            UpdateCellSize();
        }
#endif
    }
}
