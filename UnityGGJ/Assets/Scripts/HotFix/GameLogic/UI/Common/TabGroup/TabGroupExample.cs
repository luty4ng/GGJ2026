using TMPro;
using UnityEngine;

namespace GameLogic
{
    public sealed class TabGroupExample : MonoBehaviour
    {
        [SerializeField] private TabGroup m_tabGroup;
        [SerializeField] private TMP_Text m_label;

        private void Awake()
        {
            if (m_tabGroup == null)
            {
                m_tabGroup = GetComponentInChildren<TabGroup>(true);
            }
        }

        private void OnEnable()
        {
            if (m_tabGroup == null)
            {
                return;
            }

            m_tabGroup.OnTabSelected += OnTabSelected;
        }

        private void OnDisable()
        {
            if (m_tabGroup == null)
            {
                return;
            }

            m_tabGroup.OnTabSelected -= OnTabSelected;
        }

        private void OnTabSelected(int index, TabButton tab)
        {
            if (m_label == null)
            {
                return;
            }

            string name = tab != null ? tab.name : "null";
            m_label.text = $"Selected Tab: {index} ({name})";
        }
    }
}

