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
        [Tooltip("获胜所需分数（进入Fever Time）")]
        public float WinScore = 20;

        [Header("Fever Time设置")]
        [Tooltip("Fever Time持续时间（分数归零的时间，秒）")]
        public float FeverDuration = 5f;

        [Tooltip("Fever Time下的命中窗口（毫秒）")]
        public float FeverHitWindowMS = 1000f;

        [Header("引用")]
        [Tooltip("节奏控制器")]
        public RhythmController RhythmController;

        [Tooltip("玩家角色引用")]
        public GameCharacterPlayer Player;

        [Header("游戏状态（只读）")]
        [SerializeField] private float _currentScore = 0;
        [SerializeField] private bool _isGameRunning = false;
        [SerializeField] private bool _isFeverTime = false;
        [SerializeField] private float _feverTimeRemaining = 0f;

        // Fever Time 开始时的分数（用于计算倒计时）
        private float _feverStartScore = 0f;

        /// <summary>
        /// 当前分数
        /// </summary>
        public float CurrentScore => _currentScore;

        /// <summary>
        /// 游戏是否正在运行
        /// </summary>
        public bool IsGameRunning => _isGameRunning;

        /// <summary>
        /// 是否处于Fever Time状态
        /// </summary>
        public bool IsFeverTime => _isFeverTime;

        /// <summary>
        /// Fever Time剩余时间
        /// </summary>
        public float FeverTimeRemaining => _feverTimeRemaining;

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

        private void Update()
        {
            UpdateFeverTime();
        }

        /// <summary>
        /// 更新Fever Time状态
        /// </summary>
        private void UpdateFeverTime()
        {
            if (!_isFeverTime) return;

            _feverTimeRemaining -= Time.deltaTime;

            // 计算当前分数（线性倒计时归零）
            float progress = _feverTimeRemaining / FeverDuration;
            _currentScore = _feverStartScore * progress;

            // Fever Time 结束
            if (_feverTimeRemaining <= 0f)
            {
                ExitFeverTime();
            }
        }

        /// <summary>
        /// 进入Fever Time
        /// </summary>
        private void EnterFeverTime()
        {
            _isFeverTime = true;
            _feverStartScore = _currentScore;
            _feverTimeRemaining = FeverDuration;

            // 通知RhythmController进入Fever Time
            if (RhythmController != null)
            {
                RhythmController.SetFeverTime(true, FeverHitWindowMS);
            }

            // 通知Player进入Fever Time
            if (Player != null)
            {
                Player.SetFeverTime(true);
            }

            Debug.Log($"=== 进入 Fever Time ===");
        }

        /// <summary>
        /// 退出Fever Time
        /// </summary>
        private void ExitFeverTime()
        {
            _isFeverTime = false;
            _currentScore = 0f;
            _feverTimeRemaining = 0f;

            // 通知RhythmController退出Fever Time
            if (RhythmController != null)
            {
                RhythmController.SetFeverTime(false, 0f);
            }

            // 通知Player退出Fever Time
            if (Player != null)
            {
                Player.SetFeverTime(false);
            }

            Debug.Log($"=== 退出 Fever Time ===");
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
            _isFeverTime = false;
            _feverTimeRemaining = 0f;
            _feverStartScore = 0f;
            _gameStartTime = Time.time;
            GameModule.UI.ShowUIAsync<UIFevelLineWindow>(this);

            Debug.Log("游戏开始！按对应按键在正确时机击打NPC！");
        }

        /// <summary>
        /// 击打成功
        /// </summary>
        private void OnHitSuccess(GameCharacterNpc npc)
        {
            // Fever Time 下不增加分数
            if (!_isFeverTime)
            {
                _currentScore++;
                Debug.Log($"击打成功！当前分数：{_currentScore}/{WinScore}");

                // 检查是否达到分数要求，进入Fever Time
                if (_currentScore >= WinScore)
                {
                    EnterFeverTime();
                }
            }
            else
            {
                Debug.Log($"Fever Time 击打成功！剩余时间：{_feverTimeRemaining:F1}秒");
            }

            Player?.OnHitSuccess();
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

            GUI.Label(new Rect(10, 10, 300, 30), $"分数: {_currentScore:F1}/{WinScore}", style);

            if (_isFeverTime)
            {
                GUIStyle feverStyle = new GUIStyle(GUI.skin.label);
                feverStyle.fontSize = 28;
                feverStyle.normal.textColor = Color.red;
                GUI.Label(new Rect(10, 40, 400, 35), $"★ FEVER TIME ★ 剩余: {_feverTimeRemaining:F1}s", feverStyle);
            }
            else
            {
                GUI.Label(new Rect(10, 40, 300, 30), $"状态: {(_isGameRunning ? "进行中" : "已结束")}", style);
            }

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
