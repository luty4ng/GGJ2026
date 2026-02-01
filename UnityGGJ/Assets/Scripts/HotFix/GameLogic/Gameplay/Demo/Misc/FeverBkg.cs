using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class FeverBkg : MonoBehaviour
    {
        public List<GameObject> m_visuals;
        void Awake()
        {
            GameEvent.AddEventListener(GameplayEventId.OnFeverTimeStart, OnFeverTimeStart);
            GameEvent.AddEventListener(GameplayEventId.OnFeverTimeEnd, OnFeverTimeEnd);
        }

        void OnDestroy()
        {
            GameEvent.RemoveEventListener(GameplayEventId.OnFeverTimeStart, OnFeverTimeStart);
            GameEvent.RemoveEventListener(GameplayEventId.OnFeverTimeEnd, OnFeverTimeEnd);
        }

        private void OnFeverTimeStart()
        {
            transform.position = new Vector3(-35f, transform.position.y, transform.position.z);
            foreach (var visual in m_visuals)
            {
                visual.SetActive(true);
            }
            GameEvent.AddEventListener(GameplayEventId.OnBeat, OnBeatFeverTime);
        }

        private void OnFeverTimeEnd()
        {
            transform.position = new Vector3(-35f, transform.position.y, transform.position.z);
            foreach (var visual in m_visuals)
            {
                visual.SetActive(false);
            }
            GameEvent.RemoveEventListener(GameplayEventId.OnBeat, OnBeatFeverTime);
        }

        private void OnBeatFeverTime()
        {
            transform.DOMoveX(transform.position.x + 2f, 0.1f);
        }
    }
}