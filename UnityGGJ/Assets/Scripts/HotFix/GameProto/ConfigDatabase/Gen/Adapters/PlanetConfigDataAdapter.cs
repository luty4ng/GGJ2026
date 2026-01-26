
namespace GameConfig
{
    public static class PlanetConfigDataAdapter
    {
        public static PlanetConfigData FromSource(GameConfig.PlanetConfig src)
        {
            var cfg = new PlanetConfigData();
            cfg.id = src.id;
            cfg.icon = src.icon;
            cfg.localizationId = src.localizationId;
            cfg.planetName = src.planetName;
            cfg.description = src.description;
            cfg.radiusRange = src.radiusRange;
            cfg.massRange = src.massRange;
            cfg.integrityRange = src.integrityRange;
            cfg.stabilityRange = src.stabilityRange;
            cfg.fluxRange = src.fluxRange;
            cfg.presetAbilities = src.presetAbilities;
            cfg.canUseActiveAbility = src.canUseActiveAbility;
            cfg.canUsePassiveAbility = src.canUsePassiveAbility;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
