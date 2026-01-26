using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace GameConfig
{
    [ConfigSource("Ability")]
    [CreateAssetMenu(fileName = "AbilityConfig", menuName = "FoldingCosmos/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        [BoxGroup("基础信息")]
        [LabelText("技能ID")]
        [SerializeField] private int m_id;

        [BoxGroup("基础信息")]
        [LabelText("技能名称")]
        [SerializeField] private string m_abilityName;

        [BoxGroup("基础信息")]
        [LabelText("技能描述")]
        [MultiLineProperty(5)]
        [SerializeField] private string m_abilityDescription;

        [BoxGroup("基础信息")]
        [LabelText("技能类型")]
        [SerializeField] private AbilityCategory m_abilityCategory;

        [BoxGroup("基础信息")]
        [LabelText("技能属性")]
        [SerializeField] private AbilityElement m_abilityElement;

        [BoxGroup("技能逻辑")]
        [ValueDropdown("GetAbilityTypeOptions")]
        [LabelText("逻辑类型")]
        [SerializeField] private string m_abilityTypeFullName;

        [BoxGroup("技能参数")]
        [LabelText("冷却时间")]
        [SerializeField] private float m_cooldown;

        [BoxGroup("技能参数")]
        [LabelText("引导时间")]
        [SerializeField] private float m_channelTime;

        [BoxGroup("技能参数")]
        [LabelText("持续时间")]
        [SerializeField] private float m_effectTime;

        [BoxGroup("技能参数")]
        [LabelText("施放消耗")]
        [SerializeField] private float m_castCost;

        [BoxGroup("技能参数")]
        [LabelText("施放范围")]
        [SerializeField] private float m_castRange;
        [BoxGroup("技能参数")]
        [LabelText("需要目标")]
        [SerializeField] private bool m_requireTarget;

        [BoxGroup("美术表现")]
        [LabelText("图标")]
        [PreviewField(60)]
        [SerializeField] private Sprite m_icon;

        public int Id => m_id;
        public string AbilityName => m_abilityName;
        public string AbilityDescription => m_abilityDescription;
        public AbilityCategory AbilityCategory => m_abilityCategory;
        public AbilityElement AbilityElement => m_abilityElement;
        public string AbilityTypeFullName => m_abilityTypeFullName;
        public float Cooldown => m_cooldown;
        public float CastCost => m_castCost;
        public float CastRange => m_castRange;
        public bool RequireTarget => m_requireTarget;
        public float ChannelTime => m_channelTime;
        public float EffectTime => m_effectTime;
        public Sprite Icon => m_icon;
#if UNITY_EDITOR
        private ValueDropdownList<string> GetAbilityTypeOptions()
        {
            var options = new ValueDropdownList<string>();
            options.Add("<None>", string.Empty);

            var abilityTypes = GameConfigTypeManager.GetTypes()
                .Where(IsValidAbilityType)
                .OrderBy(t => t.Name);

            foreach (var type in abilityTypes)
            {
                options.Add(type.FullName);
            }
            return options;
        }

        private bool IsValidAbilityType(Type type)
        {
            if (type.IsAbstract)
                return false;
            var attrs = type.GetCustomAttributes(typeof(ConfigOptionAttribute), true);
            return attrs.Any(attr => attr is ConfigOptionAttribute configOption && configOption.Tag == "Ability");
        }
#endif
    }
}

