using UnityEngine;
using System.Collections.Generic;

namespace GameLogic
{
    /// <summary>
    /// 玩家角色 - 处理玩家输入并与轨道交互
    /// </summary>
    public class GameCharacterPlayer : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("轨道控制器列表")]
        public List<LaneController> lanes = new List<LaneController>();

        [Header("按键设置")]
        [Tooltip("老板对应按键")]
        public KeyCode bossKey = KeyCode.Alpha1;

        [Tooltip("同事对应按键")]
        public KeyCode colleagueKey = KeyCode.Alpha2;

        [Tooltip("Crush对应按键")]
        public KeyCode crushKey = KeyCode.Alpha3;

        private void Update()
        {
            HandleInput();
        }

        /// <summary>
        /// 处理玩家输入
        /// </summary>
        private void HandleInput()
        {
            if (Input.GetKeyDown(bossKey))
            {
                TryHitWithType(NpcType.Boss);
            }
            else if (Input.GetKeyDown(colleagueKey))
            {
                TryHitWithType(NpcType.Colleague);
            }
            else if (Input.GetKeyDown(crushKey))
            {
                TryHitWithType(NpcType.Crush);
            }
        }

        /// <summary>
        /// 尝试用指定类型命中NPC
        /// </summary>
        /// <param name="targetType">目标NPC类型</param>
        private void TryHitWithType(NpcType targetType)
        {
            Debug.Log($"[Player] TryHitWithType: {targetType}, lanes count: {lanes.Count}");

            if (lanes.Count == 0)
            {
                Debug.LogWarning("[Player] lanes 列表为空！请在 Inspector 中设置 LaneController 引用");
                OnHitEmpty();
                return;
            }

            bool hitAny = false;

            // 遍历所有轨道尝试命中
            foreach (var lane in lanes)
            {
                if (lane != null && lane.TryHitNpc(targetType))
                {
                    hitAny = true;
                    break; // 一次只命中一个
                }
            }

            if (hitAny)
            {
                OnHitSuccess();
            }
            else
            {
                OnHitEmpty();
            }
        }

        /// <summary>
        /// 玩家判定成功时的视觉反馈
        /// </summary>
        public void OnHitSuccess()
        {
            // TODO: 添加成功的视觉效果
            Debug.Log("Hit Success!");
        }

        /// <summary>
        /// 玩家判定失败时的视觉反馈
        /// </summary>
        public void OnHitMiss()
        {
            // TODO: 添加失败的视觉效果
            Debug.Log("Hit Miss!");
        }

        /// <summary>
        /// 玩家按下按键但没有可判定目标时的反馈
        /// </summary>
        public void OnHitEmpty()
        {
            // TODO: 添加空按的视觉效果
            Debug.Log("No target to hit!");
        }
    }
}
