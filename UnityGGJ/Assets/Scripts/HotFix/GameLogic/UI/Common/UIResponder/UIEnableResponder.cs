using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UIPointerResponder))]
public sealed class UIEnableResponder : MonoBehaviour
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
    }

    private void OnMouseEnter()
    {
        m_component.enabled = true;
    }

    private void OnMouseExit()
    {
        m_component.enabled = false;
    }

    public void OnMouseLeftDown()
    {

    }

    public void OnMouseLeftUp()
    {

    }
}
