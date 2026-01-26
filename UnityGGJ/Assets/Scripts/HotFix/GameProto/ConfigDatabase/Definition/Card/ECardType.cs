using Sirenix.OdinInspector;

namespace GameConfig
{
    public enum ECardType
    {
        [LabelText("设施牌")] Facility = 1,
        [LabelText("工程牌")] Engineering = 2,
        [LabelText("策略牌")] Strategy = 3,
        [LabelText("机会牌")] Chance = 4,
        [LabelText("障碍牌")] Challenge = 5,
    }

}