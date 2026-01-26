using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(UIPointerResponder))]
public sealed class UIColorSwitchResponder : MonoBehaviour
{
    [SerializeField] MaskableGraphic m_colorGraphicA;
    [SerializeField] MaskableGraphic m_colorGraphicB;
    private Color m_colorA;
    private Color m_colorB;
    private UIPointerResponder m_responder;
    private void Awake()
    {
        m_colorA = m_colorGraphicA.color;
        m_colorB = m_colorGraphicB.color;
        m_responder = GetComponent<UIPointerResponder>();
        m_responder.OnMouseEnter += OnMouseEnter;
        m_responder.OnMouseExit += OnMouseExit;
    }

    private void OnDestroy()
    {
        if (m_responder != null)
        {
            m_responder.OnMouseEnter -= OnMouseEnter;
            m_responder.OnMouseExit -= OnMouseExit;
        }
    }

    private void OnMouseEnter()
    {
        m_colorGraphicA.color = m_colorB;
        m_colorGraphicB.color = m_colorA;
    }

    private void OnMouseExit()
    {
        m_colorGraphicA.color = m_colorA;
        m_colorGraphicB.color = m_colorB;
    }

    public void OnMouseLeftDown()
    {

    }

    public void OnMouseLeftUp()
    {

    }
}
