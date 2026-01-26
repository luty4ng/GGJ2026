using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameConfig
{
    [ConfigSource("Card")]
    [CreateAssetMenu(fileName = "CardConfig", menuName = "FoldingCosmos/CardConfig")]
    public class CardConfig : ScriptableObject
    {
        [BoxGroup("基础信息"), HorizontalGroup("基础信息/顶部", Width = 70), PreviewField(60, ObjectFieldAlignment.Left), HideLabel, PropertyOrder(0)]
        public Sprite icon;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("ID"), PropertyOrder(1)]
        public int Id;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("卡片名称"), PropertyOrder(2)]
        public string cardName;

        [LabelText("描述")]
        [TextArea]
        public string description;

        [LabelText("所属势力")]
        [ValueDropdown("@GameConfig.DropdownHelper.GetFactionIdOptions()")]
        public int factionBelong;

        [LabelText("目标类型")]
        public ETargetType targetType = ETargetType.None;

        [LabelText("卡片类型")]
        public ECardType cardType = ECardType.Facility;
        [LabelText("消耗工作量")]
        public int costWorkload = 0;

        [LabelText("消耗资源")]
        public List<CostPair> costList = new();

        [LabelText("修饰器列表")]
        [ValueDropdown("@GameConfig.DropdownHelper.GetModifierIdOptions()")]
        public List<int> modifierIdList = new();
    }
}
