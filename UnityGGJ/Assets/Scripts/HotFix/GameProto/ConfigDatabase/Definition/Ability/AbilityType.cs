using Sirenix.OdinInspector;

namespace GameConfig
{
    public enum AbilityType
    {
        [LabelText("未使用的")] UnUsed,
        [LabelText("玩家主动")] PlayerActive,
        [LabelText("自动主动")] AutoActive,
        [LabelText("被动触发")] Passive,
        [LabelText("运营相关")] Gravity
    }

}
