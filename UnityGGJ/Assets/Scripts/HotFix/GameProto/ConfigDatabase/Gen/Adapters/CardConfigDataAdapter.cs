
namespace GameConfig
{
    public static class CardConfigDataAdapter
    {
        public static CardConfigData FromSource(GameConfig.CardConfig src)
        {
            var cfg = new CardConfigData();
            cfg.Id = src.Id;
            cfg.icon = src.icon;
            cfg.cardName = src.cardName;
            cfg.description = src.description;
            cfg.factionBelong = src.factionBelong;
            cfg.targetType = src.targetType;
            cfg.cardType = src.cardType;
            cfg.costWorkload = src.costWorkload;
            cfg.costList = src.costList;
            cfg.modifierIdList = src.modifierIdList;
            cfg.name = src.name;
            cfg.hideFlags = src.hideFlags;
            return cfg;
        }
    }
}
