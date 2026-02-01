using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using System;
using Sirenix.OdinInspector;
using UnityGameFramework.Runtime;
using System.Collections;

namespace GameLogic
{
    /// <summary>
    /// 轨道控制器 - 管理单个轨道的NPC生成和判定
    /// 每N个事件对应一个NPC，事件触发NPC移动
    /// </summary>
    public class LaneController : MonoBehaviour
    {
        #region Fields
        [Header("最大事件数")]
        public int maxEndingEventCount = 500;
        [Header("轨道设置")]
        [Tooltip("匹配此轨道的Payload字符串列表")]
        public List<string> matchedPayloads = new List<string>();

        [Tooltip("对应的键盘按键")]
        public KeyCode inputKey = KeyCode.Space;

        [Header("NPC设置")]
        [Tooltip("NPC预制体列表，每次生成随机选择一个")]
        public List<GameObject> npcPrefabs = new List<GameObject>();

        [Tooltip("NPC生成位置")]
        public Transform spawnPoint;

        [Tooltip("每个NPC需要移动的次数（每次移动对应一个事件）")]
        public int movesPerNpc = 4;

        [Header("位置节点")]
        [Tooltip("NPC移动的目标位置节点列表")]
        public List<Transform> movePositions = new List<Transform>();



        // 分配给此轨道的移动事件
        private List<KoreographyEvent> _moveEvents = new List<KoreographyEvent>();

        // 分配给此轨道的生成事件
        private List<KoreographyEvent> _spawnEvents = new List<KoreographyEvent>();

        // 当前活跃的NPC列表
        private List<GameCharacterNpc> _activeNpcs = new List<GameCharacterNpc>();

        // 主控制器引用
        private RhythmController _controller;

        // 下一个待处理移动事件的索引
        private int _pendingMoveEventIdx = 0;

        // 下一个待处理生成事件的索引
        private int _pendingSpawnEventIdx = 0;

        #endregion

        #region Events

        /// <summary>
        /// 命中成功事件
        /// </summary>
        public event Action<GameCharacterNpc> OnHitSuccess;

        /// <summary>
        /// 命中失败事件
        /// </summary>
        public event Action<GameCharacterNpc> OnHitMiss;

        /// <summary>
        /// NPC错过事件
        /// </summary>
        public event Action<GameCharacterNpc> OnNpcMissed;

        #endregion

        #region Initialization

        /// <summary>
        /// 初始化轨道
        /// </summary>
        public void Initialize(RhythmController controller)
        {
            _controller = controller;
        }

        /// <summary>
        /// 添加移动事件到轨道
        /// </summary>
        public void AddMoveEvent(KoreographyEvent evt)
        {
            _moveEvents.Add(evt);
        }

        /// <summary>
        /// 添加生成事件到轨道
        /// </summary>
        public void AddSpawnEvent(KoreographyEvent evt)
        {
            _spawnEvents.Add(evt);
        }

        /// <summary>
        /// 检查payload是否匹配此轨道
        /// </summary>
        public bool DoesMatchPayload(string payload)
        {
            foreach (var matched in matchedPayloads)
            {
                if (payload == matched)
                    return true;
            }
            return false;
        }

        #endregion

        #region Unity Lifecycle
        void OnEnable()
        {
            // GameEvent.AddEventListener(GameplayEventId.OnBeat, OnBeat);
        }
        void OnDisable()
        {
            // GameEvent.RemoveEventListener(GameplayEventId.OnBeat, OnBeat);
        }
        private bool m_isBeatSpawn = false;
        private void OnBeat()
        {
            if (m_isBeatSpawn)
            {
                StartCoroutine(SpawnNextFrame());
                Debug.Log("BeatSpawn");
            }
        }

        public IEnumerator SpawnNextFrame()
        {
            yield return null;
            OnSpawnEventTriggered(null);
        }

        private void Update()
        {
            if (_controller == null)
                return;

            // 清理已错过的NPC
            CleanupMissedNpcs();

            // 处理移动事件
            ProcessMoveEvents();

            // 处理生成事件
            ProcessSpawnEvents();

            // 注意：玩家输入现在由 GameCharacterPlayer 处理
        }

        #endregion

        #region Event Processing

        /// <summary>
        /// 处理移动事件 - 触发NPC移动
        /// </summary>
        private void ProcessMoveEvents()
        {
            int currentTime = _controller.DelayedSampleTime;

            // 处理所有已到达时间的移动事件
            while (_pendingMoveEventIdx < _moveEvents.Count &&
                   _moveEvents[_pendingMoveEventIdx].StartSample <= currentTime)
            {
                KoreographyEvent evt = _moveEvents[_pendingMoveEventIdx];
                OnMoveEventTriggered(evt);
                _pendingMoveEventIdx++;
            }

            if (_pendingMoveEventIdx >= _moveEvents.Count || _pendingMoveEventIdx >= maxEndingEventCount)
            {
                Debug.Log("[LaneController] 所有移动事件已处理完毕");
                GameModule.UI.ShowUIAsync<UISimpleResultWindow>();
                _controller.audioCom.Stop();
            }
        }

        /// <summary>
        /// 处理生成事件 - 生成新的NPC
        /// </summary>
        private void ProcessSpawnEvents()
        {
            int currentTime = _controller.DelayedSampleTime;
            // 处理所有已到达时间的生成事件
            while (_pendingSpawnEventIdx < _spawnEvents.Count &&
                   _spawnEvents[_pendingSpawnEventIdx].StartSample <= currentTime)
            {
                KoreographyEvent evt = _spawnEvents[_pendingSpawnEventIdx];
                // if (!_controller.IsFeverTime)
                OnSpawnEventTriggered(evt);
                _pendingSpawnEventIdx++;
            }
            m_isBeatSpawn = _controller.IsFeverTime;
        }

        /// <summary>
        /// 移动事件触发时的处理 - 让所有已存在的NPC移动
        /// </summary>
        private void OnMoveEventTriggered(KoreographyEvent evt)
        {
            var npcsToMove = _activeNpcs.ToArray();
            foreach (var npc in npcsToMove)
            {
                if (npc != null && !npc.IsFinished)
                {
                    npc.OnEventTriggeredMove(evt);
                }
            }
            GameEvent.Send(GameplayEventId.OnBeat);
        }

        /// <summary>
        /// 生成事件触发时的处理 - 生成新的NPC
        /// </summary>
        private void OnSpawnEventTriggered(KoreographyEvent evt)
        {
            SpawnNpc();
        }

        #endregion

        #region NPC Management

        /// <summary>
        /// 清理已完成或已错过的NPC
        /// </summary>
        private void CleanupMissedNpcs()
        {
            for (int i = _activeNpcs.Count - 1; i >= 0; i--)
            {
                var npc = _activeNpcs[i];
                if (npc == null || npc.IsFinished)
                {
                    _activeNpcs.RemoveAt(i);
                    if (npc != null && !npc.HasBeenJudged)
                    {
                        OnNpcMissed?.Invoke(npc);
                    }
                }
            }
        }

        /// <summary>
        /// 生成NPC
        /// </summary>
        private GameCharacterNpc SpawnNpc()
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            if (_activeNpcs.Count > movesPerNpc || _activeNpcs.Any(npc => npc.MoveCount == 0))
                return null;

            GameObject npcObj;
            if (npcPrefabs != null && npcPrefabs.Count > 0)
            {
                // 随机选择一个预制体
                GameObject selectedPrefab = npcPrefabs[UnityEngine.Random.Range(0, npcPrefabs.Count)];
                if (selectedPrefab != null)
                {
                    npcObj = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
                }
                else
                {
                    npcObj = CreateDefaultNpc(spawnPos);
                }
            }
            else
            {
                npcObj = CreateDefaultNpc(spawnPos);
            }

            var npc = npcObj.GetComponent<GameCharacterNpc>();
            if (npc == null)
            {
                npc = npcObj.AddComponent<GameCharacterNpc>();
            }

            // 初始化NPC（事件驱动模式），传入位置节点
            npc.InitializeWithKoreographer(_controller, movesPerNpc, movePositions);

            _activeNpcs.Add(npc);
            return npc;
        }

        /// <summary>
        /// 创建默认NPC（当没有预制体时）
        /// </summary>
        private GameObject CreateDefaultNpc(Vector3 position)
        {
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            npcObj.transform.position = position;
            npcObj.name = "NPC_Default";
            return npcObj;
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// 尝试命中指定类型的NPC
        /// </summary>
        /// <param name="targetType">目标NPC类型</param>
        /// <returns>是否成功命中</returns>
        public bool TryHitNpc(NpcType targetType)
        {
            // 调试：显示当前活跃NPC数量
            Debug.Log($"[Lane] TryHitNpc called, targetType: {targetType}, activeNpcs: {_activeNpcs.Count}");

            // 查找第一个可命中且类型匹配的NPC
            GameCharacterNpc hittableNpc = null;
            float hitOffset = 0;
            for (int i = 0; i < _activeNpcs.Count; i++)
            {
                var npc = _activeNpcs[i];
                if (npc == null) continue;

                bool isHittable = IsNpcHittable(npc, out hitOffset);
                Debug.Log($"[Lane] NPC[{i}] Type: {npc.NpcType}, IsInJudgePhase: {npc.IsInJudgePhase}, MoveCount: {npc.MoveCount}, IsHittable: {isHittable}");

                if (npc.NpcType == targetType && isHittable)
                {
                    hittableNpc = npc;
                    break;
                }
            }

            if (hittableNpc != null)
            {
                hittableNpc.OnHit();
                Debug.Log($"[Lane] 命中成功, Type: {targetType}, Offset: {hitOffset}");
                OnHitSuccess?.Invoke(hittableNpc);
                return true;
            }
            else
            {
                // 检查是否有可命中但类型不匹配的NPC
                for (int i = 0; i < _activeNpcs.Count; i++)
                {
                    if (_activeNpcs[i] != null && IsNpcHittable(_activeNpcs[i], out hitOffset))
                    {
                        Debug.Log($"[Lane] 类型不匹配, 期望: {targetType}, 实际: {_activeNpcs[i].NpcType}");
                        OnHitMiss?.Invoke(_activeNpcs[i]);
                        return false;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 尝试命中任意可命中的NPC（Fever Time下使用，无视类型）
        /// </summary>
        /// <returns>是否成功命中</returns>
        public bool TryHitAnyNpc()
        {
            Debug.Log($"[Lane] TryHitAnyNpc called (Fever Time), activeNpcs: {_activeNpcs.Count}");

            // 查找第一个可命中的NPC，无视类型
            float hitOffset = 0;
            for (int i = 0; i < _activeNpcs.Count; i++)
            {
                var npc = _activeNpcs[i];
                if (npc == null) continue;

                bool isHittable = IsNpcHittable(npc, out hitOffset);
                if (isHittable)
                {
                    npc.OnHit();
                    Debug.Log($"[Lane] Fever命中成功, Type: {npc.NpcType}, Offset: {hitOffset}");
                    OnHitSuccess?.Invoke(npc);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Hit Detection

        /// <summary>
        /// 检查NPC是否可命中
        /// </summary>
        private bool IsNpcHittable(GameCharacterNpc npc, out float offset)
        {
            offset = 0;
            if (npc == null) return false;

            // 必须处于判定阶段（最后一次移动前）
            if (!npc.IsInJudgePhase) return false;

            // 检查最后一次移动事件的时间窗口
            if (npc.LastMoveEvent == null) return false;

            // int noteTime = npc.LastMoveEvent.StartSample;
            // int curTime = _controller.DelayedSampleTime;
            // int hitWindow = _controller.HitWindowSampleWidth;
            // offset = noteTime - curTime;
            // return Mathf.Abs(noteTime - curTime) <= hitWindow;

            return npc.IsInsideHitWindow(out offset);
        }

        #endregion

        #region Restart

        /// <summary>
        /// 重启轨道
        /// </summary>
        public void Restart()
        {
            // 重置事件索引
            _pendingMoveEventIdx = 0;
            _pendingSpawnEventIdx = 0;

            // 清理所有活跃NPC
            foreach (var npc in _activeNpcs)
            {
                if (npc != null)
                {
                    npc.OnClear();
                }
            }
            _activeNpcs.Clear();
        }

        #endregion

        #region Position Nodes

        /// <summary>
        /// 在编辑器中生成位置节点
        /// </summary>
        [Button("生成位置节点")]
        public void GenerateMovePositions()
        {
            // 清除旧节点
            ClearMovePositions();

            movePositions.Clear();

            Vector3 startPos = spawnPoint != null ? spawnPoint.position : transform.position;

            for (int i = 0; i < movesPerNpc; i++)
            {
                GameObject nodeObj = new GameObject($"MovePos_{i}");
                nodeObj.transform.SetParent(transform);
                // 默认沿X轴负方向排列，每个节点间隔2米
                nodeObj.transform.position = startPos + Vector3.left * (i + 1) * 2f;
                movePositions.Add(nodeObj.transform);
            }

            Debug.Log($"[LaneController] 生成了 {movesPerNpc} 个位置节点");
        }

        /// <summary>
        /// 清除所有位置节点
        /// </summary>
        [Button("清除位置节点")]
        public void ClearMovePositions()
        {
            // 在编辑器模式下使用DestroyImmediate
            for (int i = movePositions.Count - 1; i >= 0; i--)
            {
                if (movePositions[i] != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(movePositions[i].gameObject);
                    else
#endif
                        Destroy(movePositions[i].gameObject);
                }
            }
            movePositions.Clear();
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (movePositions == null || movePositions.Count == 0) return;

            // 绘制起点
            Vector3 startPos = spawnPoint != null ? spawnPoint.position : transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(startPos, 0.3f);

            // 绘制从起点到第一个节点的线
            if (movePositions.Count > 0 && movePositions[0] != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(startPos, movePositions[0].position);
            }

            // 绘制节点和连线
            for (int i = 0; i < movePositions.Count; i++)
            {
                if (movePositions[i] == null) continue;

                // 根据节点索引设置颜色
                if (i == movePositions.Count - 2)
                {
                    // 倒数第二个节点（判定位置）用蓝色
                    Gizmos.color = Color.blue;
                }
                else if (i == movePositions.Count - 1)
                {
                    // 最后一个节点用红色
                    Gizmos.color = Color.red;
                }
                else
                {
                    // 其他节点用黄色
                    Gizmos.color = Color.yellow;
                }

                // 绘制节点球体
                Gizmos.DrawWireSphere(movePositions[i].position, 0.25f);

                // 绘制节点间的连线
                if (i < movePositions.Count - 1 && movePositions[i + 1] != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(movePositions[i].position, movePositions[i + 1].position);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // 选中时用更明显的方式绘制
            if (movePositions == null || movePositions.Count == 0) return;

            for (int i = 0; i < movePositions.Count; i++)
            {
                if (movePositions[i] == null) continue;

                // 绘制索引标签位置的小标记
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(movePositions[i].position, 0.1f);
            }
        }

        #endregion
    }
}
