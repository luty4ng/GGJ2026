using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace GameConfig
{
    [ConfigSource("FactionRelationMap", "Id")]
    [CreateAssetMenu(menuName = "FoldingCosmos/Faction/FactionRelationMap")]
    public class FactionRelationMap : ScriptableObject
    {
        [Serializable]
        public class FactionPairState
        {
            public FactionDefinition A;
            public FactionDefinition B;
            [Range(-100, 100)] public int Value;
        }
        [LabelText("关系图ID")] public int Id = 0;
        [LabelText("敌对阈值"), MinMaxSlider(-100, 100, true)] public Vector2Int Hostile;
        [LabelText("友好阈值"), MinMaxSlider(-100, 100, true)] public Vector2Int Friendly;
        public List<FactionPairState> InitialRelations = new();
        public List<FactionDefinition> Factions;
        private Dictionary<(FactionDefinition, FactionDefinition), int> _relationStates;
        private Dictionary<(FactionDefinition, FactionDefinition), FactionRelation> _currentDiscreteRelations;
        public void Init()
        {
            _relationStates = new();
            _currentDiscreteRelations = new();

            foreach (var entry in InitialRelations)
            {
                if (entry.A == null || entry.B == null)
                    continue;

                var keyAB = MakeKey(entry.A, entry.B);
                var keyBA = MakeKey(entry.B, entry.A);

                _relationStates[keyAB] = entry.Value;
                _relationStates[keyBA] = entry.Value; // 保持对称

                // 初始化离散关系（按当前 value 计算一次）
                var discrete = EvaluateRelationFromValue(entry.Value, FactionRelation.Neutral);
                _currentDiscreteRelations[keyAB] = discrete;
                _currentDiscreteRelations[keyBA] = discrete;
            }
        }

        private (FactionDefinition, FactionDefinition) MakeKey(FactionDefinition a, FactionDefinition b)
        {
            // 可以保证顺序一致（如按 Id 排序），也可以保留有向关系
            // 若你希望 AB 和 BA 完全对称，可以排序：
            // return (a.Id <= b.Id) ? (a, b) : (b, a);

            return (a, b); // 这里先按有向关系实现，然后在写入时手动写 AB/BA 两份
        }

        private int GetState(FactionDefinition a, FactionDefinition b)
        {
            var key = MakeKey(a, b);
            if (_relationStates.TryGetValue(key, out var value))
                return value;
            // 默认中立 0
            return 0;
        }

        private void SetState(FactionDefinition a, FactionDefinition b, int value)
        {
            var keyAB = MakeKey(a, b);
            var keyBA = MakeKey(b, a);

            _relationStates[keyAB] = value;
            _relationStates[keyBA] = value;
        }

        private FactionRelation GetLastDiscreteRelation(FactionDefinition a, FactionDefinition b)
        {
            var key = MakeKey(a, b);
            if (_currentDiscreteRelations.TryGetValue(key, out var r))
                return r;
            return FactionRelation.Neutral;
        }

        private void SetLastDiscreteRelation(FactionDefinition a, FactionDefinition b, FactionRelation r)
        {
            var keyAB = MakeKey(a, b);
            var keyBA = MakeKey(b, a);
            _currentDiscreteRelations[keyAB] = r;
            _currentDiscreteRelations[keyBA] = r;
        }

        /// <summary>
        /// 获取当前离散关系（Hostile/Neutral/Friendly），会考虑 IsFixed、阈值和滞回
        /// </summary>
        public FactionRelation GetEffectiveRelation(FactionDefinition a, FactionDefinition b)
        {
            var value = GetState(a, b);
            if (a.Id == b.Id)
                return FactionRelation.Friendly;
            var last = GetLastDiscreteRelation(a, b);
            var next = EvaluateRelationFromValue(value, last);
            SetLastDiscreteRelation(a, b, next);
            return next;
        }

        /// <summary>
        /// 当前是否敌对
        /// </summary>
        public bool IsHostile(FactionDefinition a, FactionDefinition b)
            => GetEffectiveRelation(a, b) == FactionRelation.Hostile;

        /// <summary>
        /// 当前是否友方
        /// </summary>
        public bool IsFriendly(FactionDefinition a, FactionDefinition b)
            => GetEffectiveRelation(a, b) == FactionRelation.Friendly;

        /// <summary>
        /// 修改两派之间的关系值（比如完成任务加好感、击杀减好感）
        /// </summary>
        public void AddRelationValue(FactionDefinition a, FactionDefinition b, int delta)
        {
            var value = GetState(a, b);
            var newValue = Mathf.Clamp(value + delta, -100, 100);
            SetState(a, b, newValue);
        }

        /// <summary>
        /// 根据数值 + 上一次关系 + 阈值，计算当前离散关系（核心逻辑）
        /// </summary>
        private FactionRelation EvaluateRelationFromValue(int value, FactionRelation last)
        {
            // 保底
            value = Mathf.Clamp(value, -100, 100);

            switch (last)
            {
                case FactionRelation.Hostile:
                    // 当前是敌对，如果好感上升到“脱离敌对”的阈值，就回到中立
                    if (value >= Hostile.y)
                        return FactionRelation.Neutral;
                    return FactionRelation.Hostile;

                case FactionRelation.Friendly:
                    // 当前是友方，如果好感降低到“脱离友好”的阈值，就回到中立
                    if (value <= Friendly.x)
                        return FactionRelation.Neutral;
                    return FactionRelation.Friendly;

                case FactionRelation.Neutral:
                default:
                    // 中立时，如果好感跌破“进入敌对”阈值 → 敌对
                    if (value <= Hostile.y)
                        return FactionRelation.Hostile;

                    // 中立时，如果好感超过“进入友好”阈值 → 友好
                    if (value >= Friendly.x)
                        return FactionRelation.Friendly;

                    // 否则继续保持中立
                    return FactionRelation.Neutral;
            }
        }
    }
}