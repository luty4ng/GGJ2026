
namespace GameConfig
{
    public static class ItemConfigDataAdapter
    {
        public static ItemConfigData FromSource(GameConfig.ItemConfig src)
        {
            var cfg = new ItemConfigData();
            cfg.Id = src.Id;
            cfg.Icon = src.Icon;
            cfg.Name = src.Name;
            cfg.Description = src.Description;
            cfg.CanSell = src.CanSell;
            cfg.Price = src.Price;
            cfg.CanStack = src.CanStack;
            cfg.MaxStack = src.MaxStack;
            cfg.Tier = src.Tier;
            cfg.PhysicalState = src.PhysicalState;
            cfg.Stats = src.Stats;
            cfg.StorageCondition = src.StorageCondition;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
