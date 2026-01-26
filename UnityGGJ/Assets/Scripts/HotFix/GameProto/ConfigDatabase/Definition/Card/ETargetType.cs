using Sirenix.OdinInspector;

namespace GameConfig
{
    public enum ETargetType
    {
        [LabelText("星系")]Galaxy = 1,
        [LabelText("星球")]Planet = 2,
        [LabelText("轨道")]Orbit = 3,
        [LabelText("无需目标")]None = 5,
    }
}