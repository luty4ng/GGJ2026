using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameLogic
{
    public class GameTimeController
    {
        private SimulationTicker m_ticker;
        public SimulationTicker Ticker => m_ticker;

        public GameTimeController(SimulationTicker ticker)
        {
            m_ticker = ticker;
            InputCmdWrapper.Instance.ListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl0, OnSpeedControl0);
            InputCmdWrapper.Instance.ListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl1, OnSpeedControl1);
            InputCmdWrapper.Instance.ListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl2, OnSpeedControl2);
            InputCmdWrapper.Instance.ListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl3, OnSpeedControl3);
        }

        ~GameTimeController()
        {
            InputCmdWrapper.Instance.UnListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl0, OnSpeedControl0);
            InputCmdWrapper.Instance.UnListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl1, OnSpeedControl1);
            InputCmdWrapper.Instance.UnListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl2, OnSpeedControl2);
            InputCmdWrapper.Instance.UnListenCmd(InputCmdKey.Group.SpeedControl, InputCmdKey.Action.SpeedControl3, OnSpeedControl3);
            m_ticker = null;
        }

        public Action<float> OnGameSpeedChanged { get; set; }
        private int m_lastSpeedLevel = 1;

        public void OnSpeedControl0()
        {
            if (m_ticker.IsPaused)
            {
                m_ticker.SetPaused(false);
                m_ticker.SetTimeScale(m_lastSpeedLevel);
                OnGameSpeedChanged?.Invoke(m_lastSpeedLevel);
                return;
            }

            m_lastSpeedLevel = Mathf.RoundToInt(m_ticker.TimeScale);
            m_ticker.SetPaused(true);
            OnGameSpeedChanged?.Invoke(0f);
        }

        public int GetSpeedLevel(float speed)
        {
            int speedInt = Mathf.RoundToInt(speed);
            if (speedInt != 4)
                return speedInt;
            return 3;
        }

        public void OnSpeedControl1()
        {
            m_lastSpeedLevel = Mathf.RoundToInt(m_ticker.TimeScale);
            m_ticker.SetPaused(false);
            m_ticker.SetTimeScale(1f);
            OnGameSpeedChanged?.Invoke(1f);
        }

        public void OnSpeedControl2()
        {
            m_lastSpeedLevel = Mathf.RoundToInt(m_ticker.TimeScale);
            m_ticker.SetPaused(false);
            m_ticker.SetTimeScale(2f);
            OnGameSpeedChanged?.Invoke(2f);
        }

        public void OnSpeedControl3()
        {
            m_lastSpeedLevel = Mathf.RoundToInt(m_ticker.TimeScale);
            m_ticker.SetPaused(false);
            m_ticker.SetTimeScale(4f);
            OnGameSpeedChanged?.Invoke(4f);
        }
    }
}