using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class EasySpriteSwitcher : MonoBehaviour
    {
        public Sprite spriteA;
        public Sprite spriteB;
        private SpriteRenderer m_spriteRenderer;
        void Awake()
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            GameEvent.AddEventListener(GameplayEventId.OnBeat, OnBeat);
        }

        void OnDestroy()
        {
            GameEvent.RemoveEventListener(GameplayEventId.OnBeat, OnBeat);
        }

        private void OnBeat()
        {
            if(m_spriteRenderer.sprite == spriteA)
                m_spriteRenderer.sprite = spriteB;
            else
                m_spriteRenderer.sprite = spriteA;
        }
    }
}