using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    #region Subscription Token

    /// <summary>
    /// 订阅句柄，用于安全解绑
    /// </summary>
    public sealed class TickSubscription : IDisposable
    {
        private Action _unsubscribe;

        internal TickSubscription(Action unsubscribe)
        {
            _unsubscribe = unsubscribe;
        }
        public void Dispose()
        {
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }

    #endregion

    #region Internal Safe Action List

    /// <summary>
    /// 支持 tick 中安全移除的 Action 列表
    /// </summary>
    internal sealed class SafeActionList<T>
    {
        private readonly List<T> _items = new();
        private readonly List<T> _toRemove = new();
        private bool _iterating;

        public void Add(T item)
        {
            if (item == null || _items.Contains(item)) return;
            _items.Add(item);
        }

        public void Remove(T item)
        {
            if (_iterating)
                _toRemove.Add(item);
            else
                _items.Remove(item);
        }

        public void ForEach(Action<T> action)
        {
            _iterating = true;
            for (int i = 0; i < _items.Count; i++)
                action(_items[i]);
            _iterating = false;

            if (_toRemove.Count > 0)
            {
                for (int i = 0; i < _toRemove.Count; i++)
                    _items.Remove(_toRemove[i]);
                _toRemove.Clear();
            }
        }

        public void Clear()
        {
            _items.Clear();
            _toRemove.Clear();
        }
    }

    #endregion

    #region 1) SimulationTicker（固定步长）

    public sealed class SimulationTicker
    {
        private readonly SafeActionList<Action<float>> _subs = new();

        private float _accumulator;

        public float FixedDelta { get; private set; }
        public float TimeScale { get; private set; } = 1f;
        public bool IsPaused { get; private set; }
        public int MaxStepsPerFrame { get; set; } = 8;

        public SimulationTicker(float fixedDelta)
        {
            FixedDelta = Mathf.Max(0.001f, fixedDelta);
        }

        public TickSubscription Subscribe(Action<float> onTick)
        {
            _subs.Add(onTick);
            return new TickSubscription(() => _subs.Remove(onTick));
        }

        public void SetPaused(bool paused) => IsPaused = paused;
        public void SetTimeScale(float scale) => TimeScale = Mathf.Max(0f, scale);
        public void SetFixedDelta(float dt) => FixedDelta = Mathf.Max(0.001f, dt);

        public void TickFrame(float unscaledDeltaTime)
        {
            if (IsPaused || TimeScale <= 0f) return;

            _accumulator += unscaledDeltaTime * TimeScale;

            int steps = 0;
            while (_accumulator >= FixedDelta)
            {
                _subs.ForEach(cb => cb(FixedDelta));
                _accumulator -= FixedDelta;

                if (++steps >= MaxStepsPerFrame)
                {
                    _accumulator = 0f;
                    break;
                }
            }
        }
    }

    #endregion

    #region 2) PresentationTicker（帧驱动）

    public sealed class PresentationTicker
    {
        private readonly SafeActionList<Action<float>> _subs = new();

        public bool FollowSimulationTimeScale { get; set; } = true;
        public float MaxScale { get; set; } = 2f;

        public TickSubscription Subscribe(Action<float> onTick)
        {
            _subs.Add(onTick);
            return new TickSubscription(() => _subs.Remove(onTick));
        }

        public void TickFrame(float deltaTime, float simScale, bool simPaused)
        {
            if (simPaused) deltaTime = 0f;

            float scale = FollowSimulationTimeScale
                ? Mathf.Min(simScale, MaxScale)
                : 1f;

            float dt = deltaTime * scale;
            _subs.ForEach(cb => cb(dt));
        }
    }

    #endregion

    #region 3) FxTicker

    public sealed class FxTicker
    {
        private readonly SafeActionList<Action<float>> _subs = new();

        public bool UseUnscaledTime { get; set; } = true;
        public bool FollowSimulationTimeScale { get; set; } = false;
        public bool PauseWithSimulation { get; set; } = false;
        public float MaxScale { get; set; } = 1.5f;

        public TickSubscription Subscribe(Action<float> onTick)
        {
            _subs.Add(onTick);
            return new TickSubscription(() => _subs.Remove(onTick));
        }

        public void TickFrame(
            float deltaTime,
            float unscaledDeltaTime,
            float simScale,
            bool simPaused)
        {
            if (PauseWithSimulation && simPaused) return;

            float baseDt = UseUnscaledTime ? unscaledDeltaTime : deltaTime;
            float scale = FollowSimulationTimeScale
                ? Mathf.Min(simScale, MaxScale)
                : 1f;

            _subs.ForEach(cb => cb(baseDt * scale));
        }
    }

    #endregion

    #region 4) UiTicker

    public sealed class UiTicker
    {
        private readonly SafeActionList<Action<float>> _subs = new();

        public TickSubscription Subscribe(Action<float> onTick)
        {
            _subs.Add(onTick);
            return new TickSubscription(() => _subs.Remove(onTick));
        }

        public void TickFrame(float unscaledDeltaTime)
        {
            _subs.ForEach(cb => cb(unscaledDeltaTime));
        }
    }

    #endregion

    #region 5) WorldTicker（低频）

    public sealed class WorldTicker
    {
        private readonly SafeActionList<Action<float>> _subs = new();
        private float _accumulator;

        public float IntervalSeconds { get; private set; }
        public bool UseUnscaledTime { get; set; } = true;
        public bool PauseWithSimulation { get; set; } = true;
        public int MaxTicksPerFrame { get; set; } = 2;

        public WorldTicker(float intervalSeconds)
        {
            IntervalSeconds = Mathf.Max(0.05f, intervalSeconds);
        }

        public TickSubscription Subscribe(Action<float> onTick)
        {
            _subs.Add(onTick);
            return new TickSubscription(() => _subs.Remove(onTick));
        }

        public void TickFrame(
            float deltaTime,
            float unscaledDeltaTime,
            bool simPaused)
        {
            if (PauseWithSimulation && simPaused) return;

            float dt = UseUnscaledTime ? unscaledDeltaTime : deltaTime;
            _accumulator += dt;

            int ticks = 0;
            while (_accumulator >= IntervalSeconds)
            {
                _subs.ForEach(cb => cb(IntervalSeconds));
                _accumulator -= IntervalSeconds;

                if (++ticks >= MaxTicksPerFrame)
                {
                    _accumulator = 0f;
                    break;
                }
            }
        }
    }

    #endregion
}
