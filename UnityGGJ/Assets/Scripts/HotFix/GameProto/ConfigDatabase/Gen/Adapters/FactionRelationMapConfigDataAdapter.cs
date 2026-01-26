
namespace GameConfig
{
    public static class FactionRelationMapConfigDataAdapter
    {
        public static FactionRelationMapConfigData FromSource(GameConfig.FactionRelationMap src)
        {
            var cfg = new FactionRelationMapConfigData();
            cfg.Id = src.Id;
            cfg.Hostile = src.Hostile;
            cfg.Friendly = src.Friendly;
            cfg.InitialRelations = src.InitialRelations;
            cfg.Factions = src.Factions;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
