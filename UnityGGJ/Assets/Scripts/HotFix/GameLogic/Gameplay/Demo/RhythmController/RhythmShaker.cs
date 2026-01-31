using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class RhythmShaker : MonoBehaviour
    {
        [Header("小幅度 Shake (OnBeat)")]
        [SerializeField] private float smallShakeStrength = 0.1f;
        [SerializeField] private float smallShakeDuration = 0.1f;
        [SerializeField] private int smallShakeVibrato = 10;
        
        [Header("大幅度 Shake (OnPlayerHitBeat)")]
        [SerializeField] private float bigShakeStrength = 0.3f;
        [SerializeField] private float bigShakeDuration = 0.2f;
        [SerializeField] private int bigShakeVibrato = 15;

        private Vector3 _originalPosition;
        private Tweener _currentShakeTween;

        private void Awake()
        {
            _originalPosition = transform.localPosition;
            GameEvent.AddEventListener(GameplayEventId.OnBeat, OnBeat);
            GameEvent.AddEventListener(GameplayEventId.OnPlayerHitBeat, OnPlayerHitBeat);
        }   

        void OnDestroy()
        {
            _currentShakeTween?.Kill();
            GameEvent.RemoveEventListener(GameplayEventId.OnBeat, OnBeat);
            GameEvent.RemoveEventListener(GameplayEventId.OnPlayerHitBeat, OnPlayerHitBeat);
        }
        
        private void OnBeat()
        {
            DoShake(smallShakeStrength, smallShakeDuration, smallShakeVibrato);
        }

        private void OnPlayerHitBeat()
        {
            DoShake(bigShakeStrength, bigShakeDuration, bigShakeVibrato);
        }

        private void DoShake(float strength, float duration, int vibrato)
        {
            // 如果有正在进行的 shake，先停止并重置位置
            if (_currentShakeTween != null && _currentShakeTween.IsActive())
            {
                _currentShakeTween.Kill();
                transform.localPosition = _originalPosition;
            }

            _currentShakeTween = transform.DOShakePosition(duration, strength, vibrato, 90f, false, true)
                .OnComplete(() => transform.localPosition = _originalPosition);
        }
    }
}