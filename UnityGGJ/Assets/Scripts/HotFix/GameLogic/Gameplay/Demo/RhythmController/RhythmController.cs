using UnityEngine;
using System.Collections.Generic;
using SonicBloom.Koreo;

namespace GameLogic
{
    /// <summary>
    /// 节奏游戏主控制器 - 基于Koreographer实现精确的音乐同步
    /// </summary>
    public class RhythmController : MonoBehaviour
    {
        #region Fields

        [Header("Koreographer设置")]
        [Tooltip("用于NPC移动的节拍事件Track ID")]
        [EventID]
        public string eventID;

        [Tooltip("用于NPC生成的节拍事件Track ID")]
        [EventID]
        public string spawnEventID;

        [Tooltip("命中判定窗口（毫秒）")]
        [Range(8f, 500f)]
        public float hitWindowRangeInMS = 80f;

        [Header("游戏设置")]
        [Tooltip("NPC每次移动的距离")]
        public float npcMoveDistance = 2f;

        [Tooltip("播放前的准备时间（秒）")]
        public float leadInTime = 1f;

        [Header("引用")]
        [Tooltip("音频播放器")]
        public AudioSource audioCom;

        [Tooltip("轨道控制器列表")]
        public List<LaneController> lanes = new List<LaneController>();

        // 当前播放的Koreography
        private Koreography _playingKoreo;

        // 命中窗口（采样数）
        private int _hitWindowRangeInSamples;

        // Lead-in时间剩余
        private float _leadInTimeLeft;

        // 播放前剩余时间
        private float _timeLeftToPlay;

        #endregion

        #region Properties

        /// <summary>
        /// 命中窗口宽度（采样数）
        /// </summary>
        public int HitWindowSampleWidth => _hitWindowRangeInSamples;

        /// <summary>
        /// 采样率
        /// </summary>
        public int SampleRate => _playingKoreo != null ? _playingKoreo.SampleRate : 44100;

        /// <summary>
        /// 当前延迟后的采样时间（考虑leadIn）
        /// </summary>
        public int DelayedSampleTime
        {
            get
            {
                if (_playingKoreo == null) return 0;
                return _playingKoreo.GetLatestSampleTime() - (int)(audioCom.pitch * _leadInTimeLeft * SampleRate);
            }
        }

        /// <summary>
        /// NPC移动距离
        /// </summary>
        public float NpcMoveDistance => npcMoveDistance;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateInternalValues();
            HandleLeadIn();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// 初始化控制器
        /// </summary>
        private void Initialize()
        {
            InitializeLeadIn();

            // 初始化所有轨道
            foreach (var lane in lanes)
            {
                lane.Initialize(this);
            }

            // 获取Koreography
            _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
            if (_playingKoreo == null)
            {
                Debug.LogError("未找到Koreography！请确保Koreographer已正确配置。");
                return;
            }

            // 获取移动节拍轨道的所有事件
            KoreographyTrackBase rhythmTrack = _playingKoreo.GetTrackByID(eventID);
            if (rhythmTrack == null)
            {
                Debug.LogError($"未找到ID为 '{eventID}' 的移动Koreography轨道！");
                return;
            }

            List<KoreographyEvent> moveEvents = rhythmTrack.GetAllEvents();
            Debug.Log($"[RhythmController] 加载了 {moveEvents.Count} 个移动节拍事件");

            // 将移动事件分发到对应的轨道
            foreach (var evt in moveEvents)
            {
                string payload = evt.GetTextValue();

                foreach (var lane in lanes)
                {
                    if (lane.DoesMatchPayload(payload))
                    {
                        lane.AddMoveEvent(evt);
                        break;
                    }
                }
            }

            // 获取生成节拍轨道的所有事件
            if (!string.IsNullOrEmpty(spawnEventID))
            {
                KoreographyTrackBase spawnTrack = _playingKoreo.GetTrackByID(spawnEventID);
                if (spawnTrack == null)
                {
                    Debug.LogWarning($"未找到ID为 '{spawnEventID}' 的生成Koreography轨道，将使用移动轨道作为生成轨道");
                }
                else
                {
                    List<KoreographyEvent> spawnEvents = spawnTrack.GetAllEvents();
                    Debug.Log($"[RhythmController] 加载了 {spawnEvents.Count} 个生成节拍事件");

                    // 将生成事件分发到对应的轨道
                    foreach (var evt in spawnEvents)
                    {
                        string payload = evt.GetTextValue();

                        foreach (var lane in lanes)
                        {
                            if (lane.DoesMatchPayload(payload))
                            {
                                lane.AddSpawnEvent(evt);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化Lead-in时间
        /// </summary>
        private void InitializeLeadIn()
        {
            if (leadInTime > 0f)
            {
                _leadInTimeLeft = leadInTime;
                _timeLeftToPlay = leadInTime - Koreographer.Instance.EventDelayInSeconds;
            }
            else
            {
                audioCom.time = -leadInTime;
                audioCom.Play();
            }
        }

        #endregion

        #region Update Logic

        /// <summary>
        /// 更新内部计算值
        /// </summary>
        private void UpdateInternalValues()
        {
            _hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);
        }

        /// <summary>
        /// 处理Lead-in倒计时
        /// </summary>
        private void HandleLeadIn()
        {
            if (_leadInTimeLeft > 0f)
            {
                _leadInTimeLeft = Mathf.Max(_leadInTimeLeft - Time.unscaledDeltaTime, 0f);
            }

            if (_timeLeftToPlay > 0f)
            {
                _timeLeftToPlay -= Time.unscaledDeltaTime;

                if (_timeLeftToPlay <= 0f)
                {
                    audioCom.time = -_timeLeftToPlay;
                    audioCom.Play();
                    _timeLeftToPlay = 0f;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 重启游戏
        /// </summary>
        public void Restart()
        {
            // 停止音频
            audioCom.Stop();
            audioCom.time = 0f;

            // 清空Koreographer延迟队列
            Koreographer.Instance.FlushDelayQueue(_playingKoreo);

            // 重置Koreography时间
            _playingKoreo.ResetTimings();

            // 重置所有轨道
            foreach (var lane in lanes)
            {
                lane.Restart();
            }

            // 重新初始化Lead-in
            InitializeLeadIn();
        }

        /// <summary>
        /// 将毫秒转换为采样数
        /// </summary>
        public int MsToSamples(float ms)
        {
            return (int)(0.001f * ms * SampleRate);
        }

        /// <summary>
        /// 将采样数转换为秒
        /// </summary>
        public float SamplesToSeconds(int samples)
        {
            return samples / (float)SampleRate;
        }

        #endregion
    }
}
