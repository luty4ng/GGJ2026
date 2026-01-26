using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameConfig
{
    public enum ItemTier
    {
        [LabelText("原始")]
        Raw,
        [LabelText("复合")]
        Composite
    }

    public enum ItemPhysicalState
    {
        [LabelText("固体")]
        Solid,
        [LabelText("液体")]
        Liquid,
        [LabelText("气体")]
        Gas,
        [LabelText("等离子体")]
        Plasma,
        [LabelText("量子")]
        Quantum,
        [LabelText("奇异")]
        Exotic
    }

    public struct ItemStats
    {
        [LabelText("结构强度"), Range(0, 5)] public int StructureStrength;
        [LabelText("化学活跃度"), Range(0, 5)] public int ChemicalActivity;
        [LabelText("场域敏感度"), Range(0, 5)] public int FieldSensitivity;
    }

    public enum ItemStorageCondition
    {
        [LabelText("无")]
        None,
        [LabelText("高温")]
        HighTemperature,
        [LabelText("低温")]
        LowTemperature,
        [LabelText("高压")]
        HightPressure,
        [LabelText("磁约束")]
        MagneticConfinement,
        [LabelText("电磁屏蔽")]
        ElectricShielding
    }


    [ConfigSource("Item")]
    [CreateAssetMenu(menuName = "FoldingCosmos/ItemConfig", fileName = "ItemConfig")]
    public class ItemConfig : ScriptableObject
    {
        [BoxGroup("基础信息"), HorizontalGroup("基础信息/顶部", Width = 70), PreviewField(60, ObjectFieldAlignment.Left), HideLabel, PropertyOrder(0)]
        public Sprite Icon;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("ID"), PropertyOrder(1)]
        public int Id;

        [BoxGroup("基础信息"), VerticalGroup("基础信息/顶部/右侧"), LabelText("名称"), PropertyOrder(2)]
        public string Name;

        [LabelText("描述"), TextArea] public string Description;

        [BoxGroup("出售设置")]
        [LabelText("是否可出售")]
        public bool CanSell = false;

        [BoxGroup("出售设置"), ShowIf("CanSell")]
        [LabelText("价格")]
        public int Price;

        [BoxGroup("堆叠设置")]
        [LabelText("是否可堆叠")]
        public bool CanStack = false;

        [BoxGroup("堆叠设置"), ShowIf("CanStack")]
        [LabelText("最大堆叠数量")]
        public int MaxStack;

        [BoxGroup("性质信息")]
        [LabelText("分类")]
        public ItemTier Tier;

        [BoxGroup("性质信息")]
        [LabelText("物理状态")]
        public ItemPhysicalState PhysicalState;

        [BoxGroup("性质信息")]
        [LabelText("参数")]
        public ItemStats Stats;

        [BoxGroup("性质信息")]
        [LabelText("存储条件")]
        public ItemStorageCondition StorageCondition;
    }
}
