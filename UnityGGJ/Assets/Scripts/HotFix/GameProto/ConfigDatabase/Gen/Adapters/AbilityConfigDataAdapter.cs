
namespace GameConfig
{
    public static class AbilityConfigDataAdapter
    {
        public static AbilityConfigData FromSource(GameConfig.AbilityConfig src)
        {
            var cfg = new AbilityConfigData();
            cfg.Id = src.Id;
            cfg.AbilityName = src.AbilityName;
            cfg.AbilityDescription = src.AbilityDescription;
            cfg.AbilityCategory = src.AbilityCategory;
            cfg.AbilityElement = src.AbilityElement;
            cfg.AbilityTypeFullName = src.AbilityTypeFullName;
            cfg.Cooldown = src.Cooldown;
            cfg.CastCost = src.CastCost;
            cfg.CastRange = src.CastRange;
            cfg.RequireTarget = src.RequireTarget;
            cfg.ChannelTime = src.ChannelTime;
            cfg.EffectTime = src.EffectTime;
            cfg.Icon = src.Icon;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
