namespace GameLogic
{
    /// <summary>
    /// 纯逻辑时间编排器（非 Mono）
    /// </summary>
    public sealed class GameTickerManager
    {
        public SimulationTicker Simulation { get; }
        public PresentationTicker Presentation { get; }
        public FxTicker Fx { get; }
        public UiTicker UI { get; }
        public WorldTicker World { get; }

        public GameTickerManager(
            float simulationFixedDelta = 1f / 30f,
            float worldIntervalSeconds = 1f)
        {
            Simulation = new SimulationTicker(simulationFixedDelta);
            Presentation = new PresentationTicker();
            Fx = new FxTicker();
            UI = new UiTicker();
            World = new WorldTicker(worldIntervalSeconds)
            {
                PauseWithSimulation = true
            };
        }

        public void Tick(float deltaTime, float unscaledDeltaTime)
        {
            Simulation.TickFrame(unscaledDeltaTime);

            Presentation.TickFrame(
                deltaTime,
                Simulation.TimeScale,
                Simulation.IsPaused);

            Fx.TickFrame(
                deltaTime,
                unscaledDeltaTime,
                Simulation.TimeScale,
                Simulation.IsPaused);

            UI.TickFrame(unscaledDeltaTime);

            World.TickFrame(
                deltaTime,
                unscaledDeltaTime,
                Simulation.IsPaused);
        }

        #region Control

        public void SetSimulationPaused(bool paused)
            => Simulation.SetPaused(paused);

        public void SetSimulationTimeScale(float scale)
            => Simulation.SetTimeScale(scale);

        public void SetSimulationFixedDelta(float fixedDelta)
            => Simulation.SetFixedDelta(fixedDelta);

        #endregion
    }
}
