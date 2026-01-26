using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [RequireComponent(typeof(Button))]
    public sealed class TabButton : MonoBehaviour
    {
        [SerializeField] private TabGroup m_group;
        [SerializeField] private int m_index = -1;
        [SerializeField] private GameObject m_targetPage;
        [SerializeField] private GameObject m_selectedIndicator;
        [SerializeField] private bool m_registerOnEnable = true;

        private Button m_button;

        public int Index => m_index;
        public GameObject TargetPage => m_targetPage;

        private void Awake()
        {
            m_button = GetComponent<Button>();
            m_button.onClick.AddListener(OnClicked);
        }

        private void OnEnable()
        {
            if (!m_registerOnEnable)
            {
                return;
            }

            if (m_group == null)
            {
                return;
            }

            m_group.RegisterTab(this);
        }

        private void OnDisable()
        {
            if (!m_registerOnEnable)
            {
                return;
            }

            if (m_group == null)
            {
                return;
            }

            m_group.UnregisterTab(this);
        }

        private void OnDestroy()
        {
            if (m_button == null)
            {
                return;
            }

            m_button.onClick.RemoveListener(OnClicked);
        }

        public void SetGroup(TabGroup group)
        {
            m_group = group;
        }

        public void SetIndex(int index)
        {
            m_index = index;
        }

        public void ApplySelected(bool selected)
        {
            if (m_selectedIndicator != null)
            {
                m_selectedIndicator.SetActive(selected);
            }

            if (m_button != null)
            {
                m_button.interactable = !selected;
            }
        }

        private void OnClicked()
        {
            if (m_group == null)
            {
                return;
            }

            m_group.Select(this);
        }
    }
}

