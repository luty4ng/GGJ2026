using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameConfig
{
    [Serializable]
    public struct AbilityAndStragtegy
    {
        [LabelText("技能")]
        [ValueDropdown("@GameConfig.DropdownHelper.GetAbilityIdOptions()")]
        public int abilityId;

        [LabelText("瞄准策略")]
        [ValueDropdown("@GameConfig.DropdownHelper.GetStrategyTypeOptions()")]
        public string strategyType;
    }
    
    [ConfigSource("Planet", "id")]
    [CreateAssetMenu(menuName = "FoldingCosmos/PlanetConfig", fileName = "PlanetConfig")]
    public class PlanetConfig : ScriptableObject
    {
        [BoxGroup("基础信息"), HorizontalGroup("基础信息/顶部", Width = 70), PreviewField(60, ObjectFieldAlignment.Left), HideLabel, PropertyOrder(0)]
        public Sprite icon;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("ID"), PropertyOrder(1)]
        public int id;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("本地化ID"), PropertyOrder(2)]
        public string localizationId;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("星球名称"), PropertyOrder(2)]
        public string planetName;

        [BoxGroup("基础信息"), LabelText("描述"), TextArea(4, 10), PropertyOrder(3)]
        public string description;

        [FoldoutGroup("属性范围"), LabelText("半径范围"), MinMaxSlider(0.5f, 5f, true), PropertyOrder(9)]
        public Vector2 radiusRange = new Vector2(0.5f, 2f);

        [FoldoutGroup("属性范围"), LabelText("质量范围"), MinMaxSlider(1, 15, true), PropertyOrder(10)]
        public Vector2Int massRange = new Vector2Int(5, 5);

        [FoldoutGroup("属性范围"), LabelText("完整度范围"), MinMaxSlider(1, 15, true), PropertyOrder(11)]
        public Vector2Int integrityRange = new Vector2Int(5, 5);

        [FoldoutGroup("属性范围"), LabelText("稳定性范围"), MinMaxSlider(1, 15, true), PropertyOrder(12)]
        public Vector2Int stabilityRange = new Vector2Int(5, 5);

        [FoldoutGroup("属性范围"), LabelText("通量范围"), MinMaxSlider(1, 15, true), PropertyOrder(13)]
        public Vector2Int fluxRange = new Vector2Int(5, 5);

        [FoldoutGroup("固定属性"), LabelText("技能配置"), PropertyOrder(23)]
        public List<AbilityAndStragtegy> presetAbilities = new List<AbilityAndStragtegy>();

        [FoldoutGroup("功能配置"), LabelText("可使用主动技能"), PropertyOrder(30)]
        public bool canUseActiveAbility = true;

        [FoldoutGroup("功能配置"), LabelText("可使用被动技能"), PropertyOrder(31)]
        public bool canUsePassiveAbility = true;
    }
}
