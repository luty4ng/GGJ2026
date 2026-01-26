using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    public class UITabResponderGroup : MonoBehaviour
    {
        private readonly List<UITabResponder> m_responders = new List<UITabResponder>();

        /// <summary>
        /// 注册一个Tab响应器（主动注册式）
        /// </summary>
        public void RegisterResponder(UITabResponder responder)
        {
            if (responder == null)
            {
                return;
            }

            // 避免重复注册
            if (m_responders.Contains(responder))
            {
                return;
            }

            int index = m_responders.Count;
            responder.SetGroup(this, index);
            m_responders.Add(responder);
        }

        /// <summary>
        /// 注销一个Tab响应器
        /// </summary>
        public void UnregisterResponder(UITabResponder responder)
        {
            if (responder == null)
            {
                return;
            }

            int index = m_responders.IndexOf(responder);
            if (index < 0)
            {
                return;
            }

            m_responders.RemoveAt(index);

            // 重新分配索引
            for (int i = index; i < m_responders.Count; i++)
            {
                if (m_responders[i] != null)
                {
                    m_responders[i].SetGroup(this, i);
                }
            }

            // 如果删除的是当前选中的，需要调整
            if (m_currentIndex == index)
            {
                m_currentIndex = -1;
            }
            else if (m_currentIndex > index)
            {
                m_currentIndex--;
            }
        }

        public void Select(UITabResponder responder)
        {
            if (responder == null)
            {
                return;
            }

            if (responder.Index < 0)
            {
                int index = m_responders.IndexOf(responder);
                if (index < 0)
                {
                    return;
                }

                SelectInternal(index);
                return;
            }

            SelectInternal(responder.Index);
        }

        private int m_currentIndex = -1;
        private void SelectInternal(int index)
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
            ApplyResponderSelection(m_currentIndex);
        }

        private int ClampIndex(int index)
        {
            if (m_responders.Count <= 0)
            {
                return -1;
            }

            if (index < 0)
            {
                return 0;
            }

            if (index >= m_responders.Count)
            {
                return m_responders.Count - 1;
            }

            return index;
        }

        private void ApplyResponderSelection(int selectedIndex)
        {
            for (int i = 0; i < m_responders.Count; i++)
            {
                var responder = m_responders[i];
                if (responder == null)
                {
                    continue;
                }
                responder.SetSelected(i == selectedIndex);
            }
        }
    }
}