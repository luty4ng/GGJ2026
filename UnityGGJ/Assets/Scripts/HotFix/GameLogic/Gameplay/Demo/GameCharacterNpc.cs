using UnityEngine;
using System;
using System.Collections.Generic;
using SonicBloom.Koreo;

namespace GameLogic
{
    /// <summary>
    /// NPC类型枚举
    /// </summary>
    public enum NpcType
    {
        Boss = 1,       // 老板 - 按键1
        Colleague = 2,  // 同事 - 按键2
        Crush = 3       // Crush - 按键3
    }

    /// <summary>
    /// NPC角色 - 由LaneController基于Koreographer事件触发移动
    /// </summary>
    public class GameCharacterNpc : MonoBehaviour
    {
        #region Fields

        /// <summary>
        /// NPC类型（老板、同事、Crush）
        /// </summary>
        [SerializeField]
        private NpcType _npcType = NpcType.Boss;

        [SerializeField]
        private SpriteRenderer m_spriteRenderer;
        [SerializeField]
        private Sprite m_cutImage;
        [SerializeField]
        private Sprite m_normalImage;
        [SerializeField]
        private List<Sprite> m_happyImages;
        [SerializeField]
        private List<Sprite> m_angryImages;

        /// <summary>
        /// 获取NPC类型
        /// </summary>
        public NpcType NpcType => _npcType;
        /// <summary>
        /// 移动目标位置节点列表
        /// </summary>
        private List<Transform> _movePositions;

        /// <summary>
        /// 最后一次移动对应的事件（用于判定）
        /// </summary>
        public KoreographyEvent LastMoveEvent { get; private set; }

        /// <summary>
        /// 判定事件（最后一次移动时的事件）
        /// </summary>
        public KoreographyEvent JudgeEvent { get; private set; }

        /// <summary>
        /// 节奏控制器引用
        /// </summary>
        private RhythmController _rhythmController;

        /// <summary>
        /// 需要移动的总次数
        /// </summary>
        public int TotalMoveCount { get; private set; } = 4;

        /// <summary>
        /// 当前移动次数
        /// </summary>
        public int MoveCount { get; private set; } = 0;

        /// <summary>
        /// 是否处于可判定状态（倒数第二个窗口，即第 TotalMoveCount-1 次移动后）
        /// </summary>
        public bool IsInJudgePhase => MoveCount == TotalMoveCount - 1;

        /// <summary>
        /// 是否已完成所有移动
        /// </summary>
        public bool IsFinished => MoveCount >= TotalMoveCount;

        /// <summary>
        /// 是否已经被判定过
        /// </summary>
        public bool HasBeenJudged { get; private set; } = false;

        /// <summary>
        /// 是否判定成功
        /// </summary>
        public bool JudgeSuccess { get; private set; } = false;

        /// <summary>
        /// 当NPC需要被销毁时触发
        /// </summary>
        public event Action<GameCharacterNpc, bool> OnNpcFinished;

        private bool _isActive = true;

        #endregion

        #region Initialization

        /// <summary>
        /// 使用Koreographer初始化NPC（事件驱动模式）
        /// </summary>
        /// <param name="controller">节奏控制器</param>
        /// <param name="totalMoves">需要移动的总次数</param>
        /// <param name="movePositions">移动目标位置节点列表</param>
        public void InitializeWithKoreographer(RhythmController controller, int totalMoves = 4, List<Transform> movePositions = null)
        {
            _rhythmController = controller;
            TotalMoveCount = totalMoves;
            MoveCount = 0;
            _movePositions = movePositions;
        }

        #endregion

        #region Beat Handling

        /// <summary>
        /// 由外部事件触发的移动
        /// </summary>
        /// <param name="evt">触发此次移动的Koreography事件</param>
        public void OnEventTriggeredMove(KoreographyEvent evt)
        {
            if (!_isActive || IsFinished) return;

            MoveCount++;
            LastMoveEvent = evt;
            PerformMove();

            // 最后一个窗口：显示结果颜色
            if (MoveCount == TotalMoveCount)
            {
                JudgeEvent = evt;

                // TODO: 临时方法 - 根据判定结果改变颜色
                ApplyResultColor();
            }

            // 如果已经移动完所有次数，准备销毁
            if (IsFinished)
            {
                FinishNpc();
            }
        }

        /// <summary>
        /// 执行移动 - 移动到对应的位置节点
        /// </summary>
        private void PerformMove()
        {
            // 使用位置节点移动
            if (_movePositions != null && _movePositions.Count > 0)
            {
                // MoveCount 从1开始，所以索引是 MoveCount - 1
                int targetIndex = MoveCount - 1;
                if (targetIndex >= 0 && targetIndex < _movePositions.Count && _movePositions[targetIndex] != null)
                {
                    transform.position = _movePositions[targetIndex].position;
                }
                else
                {
                    // 如果节点不够，使用最后一个节点
                    var lastNode = _movePositions[_movePositions.Count - 1];
                    if (lastNode != null)
                    {
                        transform.position = lastNode.position;
                    }
                }
            }
        }

        #endregion

        #region Hit Detection

        /// <summary>
        /// 命中成功（由LaneController调用）
        /// 命中后不会删除NPC，等到最后一个窗口时根据结果显示颜色
        /// </summary>
        public void OnHit()
        {
            HasBeenJudged = true;
            JudgeSuccess = true;
            Debug.Log($"[NPC] 命中成功！MoveCount: {MoveCount}");
        }

        /// <summary>
        /// 清除NPC（由LaneController调用）
        /// </summary>
        public void OnClear()
        {
            _isActive = false;
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Finish

        /// <summary>
        /// 结束NPC
        /// </summary>
        private void FinishNpc()
        {
            _isActive = false;
            OnNpcFinished?.Invoke(this, JudgeSuccess);

            // 延迟销毁，给视觉反馈留时间
            Destroy(gameObject, 0.5f);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// TODO: 临时方法 - 根据判定结果改变颜色
        /// 命中成功显示绿色，失败显示红色
        /// </summary>
        private void ApplyResultColor()
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // 创建新材质实例避免影响其他对象
                Material mat = renderer.material;
                if (JudgeSuccess)
                {
                    mat.color = Color.green;
                    Debug.Log("[NPC] 判定成功 - 绿色");
                }
                else
                {
                    mat.color = Color.red;
                    Debug.Log("[NPC] 判定失败 - 红色");
                }
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// 获取当前判定的时间偏差（用于显示判定精度）
        /// </summary>
        public float GetTimingOffset()
        {
            if (_rhythmController != null && LastMoveEvent != null)
            {
                int sampleOffset = LastMoveEvent.StartSample - _rhythmController.DelayedSampleTime;
                return _rhythmController.SamplesToSeconds(sampleOffset);
            }
            return 0f;
        }

        #endregion
    }
}
