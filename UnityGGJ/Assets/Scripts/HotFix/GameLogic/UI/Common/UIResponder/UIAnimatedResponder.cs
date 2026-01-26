using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UIPointerResponder))]
public sealed class UIAnimatedResponder : MonoBehaviour
{
    [SerializeField, LabelText("悬停缩放倍数")] private float m_hoverScale = 1.1f;
    [SerializeField, LabelText("悬停动画时长")] private float m_hoverDuration = 0.2f;
    [SerializeField, LabelText("悬停动画缓动")] private Ease m_hoverEase = Ease.OutQuad;
    
    [SerializeField, LabelText("点击弹跳缩放倍数")] private float m_clickBounceScale = 0.95f;
    [SerializeField, LabelText("点击弹跳动画时长")] private float m_clickBounceDuration = 0.1f;
    [SerializeField, LabelText("点击弹跳动画缓动")] private Ease m_clickBounceEase = Ease.OutQuad;

    private RectTransform m_animTransform;
    private UIPointerResponder m_responder;
    private Vector3 m_originalScale;
    private Tween m_currentTween;

    private void Awake()
    {
        m_animTransform = GetComponent<RectTransform>();
        m_originalScale = m_animTransform.localScale;
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
        m_currentTween?.Kill();
    }

    private void OnMouseEnter()
    {
        m_currentTween?.Kill();
        m_currentTween = m_animTransform.DOScale(m_originalScale * m_hoverScale, m_hoverDuration)
            .SetEase(m_hoverEase);
    }

    private void OnMouseExit()
    {
        m_currentTween?.Kill();
        m_currentTween = m_animTransform.DOScale(m_originalScale, m_hoverDuration)
            .SetEase(m_hoverEase);
    }

    public void OnMouseLeftDown()
    {
        m_currentTween?.Kill();
        Sequence bounceSequence = DOTween.Sequence();
        bounceSequence.Append(m_animTransform.DOScale(m_originalScale * m_clickBounceScale, m_clickBounceDuration)
            .SetEase(m_clickBounceEase));
        bounceSequence.Append(m_animTransform.DOScale(m_originalScale * m_hoverScale, m_clickBounceDuration)
            .SetEase(m_clickBounceEase));
        m_currentTween = bounceSequence;
    }

    public void OnMouseLeftUp()
    {
    }
}
