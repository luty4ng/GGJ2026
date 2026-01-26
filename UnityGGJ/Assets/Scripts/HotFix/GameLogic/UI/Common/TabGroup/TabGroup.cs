using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public sealed class TabGroup : MonoBehaviour
    {
        [SerializeField] private Transform m_tabsRoot;
        [SerializeField] private Transform m_pagesRoot;
        [SerializeField] private bool m_autoCollectTabs = true;
        [SerializeField] private bool m_selectOnEnable = true;
        [SerializeField] private int m_defaultIndex;

        private readonly List<TabButton> m_tabs = new List<TabButton>();
        private int m_currentIndex = -1;

        public event Action<int, TabButton> OnTabSelected;

        public int CurrentIndex => m_currentIndex;

        private void Awake()
        {
            if (m_tabsRoot == null)
            {
                m_tabsRoot = transform;
            }

            if (m_autoCollectTabs)
            {
                CollectTabsFromRoot();
            }
        }

        private void OnEnable()
        {
            if (m_selectOnEnable)
            {
                Select(ClampIndex(m_defaultIndex));
            }
        }

        private void OnDisable()
        {
            ApplyTabSelection(-1);
            ApplyPageVisibility(-1);
            m_currentIndex = -1;
        }

        public void CollectTabsFromRoot()
        {
            m_tabs.Clear();
            if (m_tabsRoot == null)
            {
                return;
            }

            var tabs = m_tabsRoot.GetComponentsInChildren<TabButton>(true);
            for (int i = 0; i < tabs.Length; i++)
            {
                RegisterTabInternal(tabs[i], i);
            }
        }

        public void RegisterTab(TabButton tab)
        {
            if (tab == null)
            {
                return;
            }

            if (m_tabs.Contains(tab))
            {
                return;
            }

            m_tabs.Add(tab);
            RegisterTabInternal(tab, m_tabs.Count - 1);
        }

        public void UnregisterTab(TabButton tab)
        {
            if (tab == null)
            {
                return;
            }

            int index = m_tabs.IndexOf(tab);
            if (index < 0)
            {
                return;
            }

            m_tabs.RemoveAt(index);
            RefreshTabIndices();

            if (m_currentIndex == index)
            {
                m_currentIndex = -1;
                Select(ClampIndex(m_defaultIndex));
                return;
            }

            if (m_currentIndex > index)
            {
                m_currentIndex--;
            }

            ApplyTabSelection(m_currentIndex);
            ApplyPageVisibility(m_currentIndex);
        }

        public void RefreshTabIndices()
        {
            for (int i = 0; i < m_tabs.Count; i++)
            {
                if (m_tabs[i] == null)
                {
                    continue;
                }

                m_tabs[i].SetIndex(i);
                m_tabs[i].SetGroup(this);
            }
        }

        public void Select(int index)
        {
            index = ClampIndex(index);
            if (index < 0)
            {
                return;
            }

            if (m_currentIndex == index)
            {
                return;
            }

            m_currentIndex = index;
            ApplyTabSelection(m_currentIndex);
            ApplyPageVisibility(m_currentIndex);
            OnTabSelected?.Invoke(m_currentIndex, GetTab(m_currentIndex));
        }

        public void Select(TabButton tab)
        {
            if (tab == null)
            {
                return;
            }

            if (tab.Index < 0)
            {
                int index = m_tabs.IndexOf(tab);
                if (index < 0)
                {
                    return;
                }

                Select(index);
                return;
            }

            Select(tab.Index);
        }

        public TabButton GetTab(int index)
        {
            if (index < 0 || index >= m_tabs.Count)
            {
                return null;
            }

            return m_tabs[index];
        }

        private void RegisterTabInternal(TabButton tab, int index)
        {
            if (tab == null)
            {
                return;
            }

            tab.SetGroup(this);
            tab.SetIndex(index);
            tab.ApplySelected(false);
        }

        private int ClampIndex(int index)
        {
            if (m_tabs.Count <= 0)
            {
                return -1;
            }

            if (index < 0)
            {
                return 0;
            }

            if (index >= m_tabs.Count)
            {
                return m_tabs.Count - 1;
            }

            return index;
        }

        private void ApplyTabSelection(int selectedIndex)
        {
            for (int i = 0; i < m_tabs.Count; i++)
            {
                var tab = m_tabs[i];
                if (tab == null)
                {
                    continue;
                }

                tab.ApplySelected(i == selectedIndex);
            }
        }

        private void ApplyPageVisibility(int selectedIndex)
        {
            bool usedPerTabTarget = false;
            for (int i = 0; i < m_tabs.Count; i++)
            {
                var tab = m_tabs[i];
                if (tab == null)
                {
                    continue;
                }

                if (tab.TargetPage == null)
                {
                    continue;
                }

                usedPerTabTarget = true;
                tab.TargetPage.SetActive(i == selectedIndex);
            }

            if (usedPerTabTarget)
            {
                return;
            }

            if (m_pagesRoot == null)
            {
                return;
            }

            int childCount = m_pagesRoot.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var page = m_pagesRoot.GetChild(i);
                if (page == null)
                {
                    continue;
                }

                page.gameObject.SetActive(i == selectedIndex);
            }
        }
    }
}
