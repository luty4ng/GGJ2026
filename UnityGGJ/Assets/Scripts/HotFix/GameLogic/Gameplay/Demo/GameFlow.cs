using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 2D节奏游戏主流程控制器
    /// 节奏控制由RhythmController和LaneController负责
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        [Header("游戏设置")]
        [Tooltip("获胜所需分数")]
        public int WinScore = 20;

        [Header("引用")]
        [Tooltip("节奏控制器")]
        public RhythmController RhythmController;

        [Tooltip("玩家角色引用")]
        public GameCharacterPlayer Player;

        [Header("游戏状态（只读）")]
        [SerializeField] private int _currentScore = 0;
        [SerializeField] private bool _isGameRunning = false;

        /// <summary>
        /// 当前分数
        /// </summary>
        public int CurrentScore => _currentScore;

        /// <summary>
        /// 游戏是否正在运行
        /// </summary>
        public bool IsGameRunning => _isGameRunning;

        /// <summary>
        /// 游戏开始时间
        /// </summary>
        private float _gameStartTime;

        private void Start()
        {
            // 订阅LaneController的事件
            SubscribeLaneEvents();
            StartGame();
        }

        /// <summary>
        /// 订阅所有轨道的事件
        /// </summary>
        private void SubscribeLaneEvents()
        {
            if (RhythmController == null) return;

            foreach (var lane in RhythmController.lanes)
            {
                lane.OnHitSuccess += OnHitSuccess;
                lane.OnHitMiss += OnHitMiss;
                lane.OnNpcMissed += OnNpcMissed;
            }
        }

        /// <summary>
        /// 取消订阅所有轨道的事件
        /// </summary>
        private void UnsubscribeLaneEvents()
        {
            if (RhythmController == null) return;

            foreach (var lane in RhythmController.lanes)
            {
                lane.OnHitSuccess -= OnHitSuccess;
                lane.OnHitMiss -= OnHitMiss;
                lane.OnNpcMissed -= OnNpcMissed;
            }
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
        {
            _currentScore = 0;
            _isGameRunning = true;
            _gameStartTime = Time.time;

            Debug.Log("游戏开始！按对应按键在正确时机击打NPC！");
        }

        /// <summary>
        /// 击打成功
        /// </summary>
        private void OnHitSuccess(GameCharacterNpc npc)
        {
            _currentScore++;
            Player?.OnHitSuccess();

            Debug.Log($"击打成功！当前分数：{_currentScore}/{WinScore}");

            // 检查是否达到胜利条件
            if (_currentScore >= WinScore)
            {
                OnGameWin();
            }
        }

        /// <summary>
        /// 击打失败（时机不对）
        /// </summary>
        private void OnHitMiss(GameCharacterNpc npc)
        {
            Player?.OnHitMiss();
            Debug.Log($"击打失败！时机不对。当前分数：{_currentScore}/{WinScore}");
        }

        /// <summary>
        /// NPC被错过
        /// </summary>
        private void OnNpcMissed(GameCharacterNpc npc)
        {
            Debug.Log("错过了一个NPC！");
        }

        /// <summary>
        /// 游戏胜利
        /// </summary>
        private void OnGameWin()
        {
            _isGameRunning = false;
            float totalTime = Time.time - _gameStartTime;

            Debug.Log($"=== 游戏结束 ===");
            Debug.Log($"最终分数：{_currentScore}");
            Debug.Log($"游戏时长：{totalTime:F1}秒");

            // TODO: 显示结算界面，后续扩展
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        public void RestartGame()
        {
            if (RhythmController != null)
            {
                RhythmController.Restart();
            }
            StartGame();
        }

        private void OnDestroy()
        {
            UnsubscribeLaneEvents();
        }

        #region Debug辅助

        private void OnGUI()
        {
            // 简单的调试UI
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 24;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 300, 30), $"分数: {_currentScore}/{WinScore}", style);
            GUI.Label(new Rect(10, 40, 300, 30), $"状态: {(_isGameRunning ? "进行中" : "已结束")}", style);

            if (!_isGameRunning)
            {
                if (GUI.Button(new Rect(10, 80, 150, 40), "重新开始"))
                {
                    RestartGame();
                }
            }
        }

        #endregion
    }
}
