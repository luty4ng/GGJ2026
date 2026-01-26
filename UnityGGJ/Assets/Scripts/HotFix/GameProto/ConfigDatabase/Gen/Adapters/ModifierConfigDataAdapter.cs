
namespace GameConfig
{
    public static class ModifierConfigDataAdapter
    {
        public static ModifierConfigData FromSource(GameConfig.ModifierConfig src)
        {
            var cfg = new ModifierConfigData();
            cfg.Id = src.Id;
            cfg.Duration = src.Duration;
            cfg.MaxStack = src.MaxStack;
            cfg.StackRule = src.StackRule;
            cfg.Tags = src.Tags;
            cfg.OpType = src.OpType;
            cfg.ValueOps = src.ValueOps;
            cfg.UseSpecLogic = src.UseSpecLogic;
            cfg.LogicType = src.LogicType;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
