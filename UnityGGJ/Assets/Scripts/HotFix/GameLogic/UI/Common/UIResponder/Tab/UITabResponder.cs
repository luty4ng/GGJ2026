using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace GameLogic
{
    [RequireComponent(typeof(UIPointerResponder))]
    public sealed class UITabResponder : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour m_component;
        private UIPointerResponder m_responder;

        private void Awake()
        {
            m_responder = GetComponent<UIPointerResponder>();
            m_responder.OnMouseEnter += OnMouseEnter;
            m_responder.OnMouseExit += OnMouseExit;
            m_responder.OnMouseLeftDown += OnMouseLeftDown;
            m_responder.OnMouseLeftUp += OnMouseLeftUp;
            SetSelected(false);
            
            // 主动注册到Group
            RegisterToGroup();
        }

        private void OnDestroy()
        {
            if (m_responder != null)
            {
                m_responder.OnMouseEnter -= OnMouseEnter;
                m_responder.OnMouseExit -= OnMouseExit;
                m_responder.OnMouseLeftDown -= OnMouseLeftDown;
                m_responder.OnMouseLeftUp -= OnMouseLeftUp;
            }
            
            // 从Group注销
            UnregisterFromGroup();
        }

        private UITabResponderGroup m_group;
        private int m_index;
        public int Index => m_index;
        private bool m_selected;
        
        /// <summary>
        /// 主动从父节点查找或创建Group，并注册自己
        /// </summary>
        private void RegisterToGroup()
        {
            // 从父节点查找Group
            UITabResponderGroup group = transform.parent?.GetComponentInParent<UITabResponderGroup>();
            
            // 如果没有找到，就在父节点上添加一个
            if (group == null && transform.parent != null)
            {
                group = transform.parent.gameObject.AddComponent<UITabResponderGroup>();
            }
            
            // 注册到Group
            if (group != null)
            {
                group.RegisterResponder(this);
            }
        }
        
        /// <summary>
        /// 从Group注销
        /// </summary>
        private void UnregisterFromGroup()
        {
            if (m_group != null)
            {
                m_group.UnregisterResponder(this);
                m_group = null;
            }
        }
        
        public void SetGroup(UITabResponderGroup group, int indexInGroup)
        {
            m_group = group;
            m_index = indexInGroup;
            m_selected = false;
        }
        public void SetSelected(bool selected)
        {
            m_component.enabled = selected;
            m_selected = selected;
        }

        private void OnMouseEnter()
        {
            if(m_selected)
                return;
            m_component.enabled = true;
        }

        private void OnMouseExit()
        {
            if(m_selected)
                return;
            m_component.enabled = false;
        }

        public void OnMouseLeftDown()
        {
            if(m_group == null)
                return;
            m_group.Select(this);
        }

        public void OnMouseLeftUp()
        {

        }
    }
}
