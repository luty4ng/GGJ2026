using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameConfig
{
    public enum ModifierOp
    {
        Add,
        Mul,
    }

    public enum ModifierOpType
    {
        [LabelText("链式增量")] IncrementalBase,
        [LabelText("公式重建")] RebuildFromeBase
    }

    public enum ModifierStackRule
    {
        AddStack,         // 增加层数
        Refresh,  // 叠加时刷新持续时间
        Ignore            // 已存在则忽略
    }

    [Serializable]
    public struct ModifierKeyValueOp
    {
        public string Key;
        public float Value;
        public ModifierOp Op;
    }

    [ConfigSource("Modifier")]
    [CreateAssetMenu(menuName = "FoldingCosmos/ModifierConfig", fileName = "ModifierConfig")]
    public class ModifierConfig : ScriptableObject
    {
        [LabelText("ID")] public int Id;
        [LabelText("持续时间")] public float Duration = 0f;
        [LabelText("最大层数")] public int MaxStack = 0;
        [LabelText("堆叠规则")] public ModifierStackRule StackRule = ModifierStackRule.AddStack;
        [LabelText("标签")] public List<string> Tags = new List<string>();
        [BoxGroup("数值操作")]
        [LabelText("操作类型")] public ModifierOpType OpType = ModifierOpType.IncrementalBase;
        [BoxGroup("数值操作")]
        [LabelText("操作列表")] public List<ModifierKeyValueOp> ValueOps = new List<ModifierKeyValueOp>();

        [BoxGroup("逻辑配置")]
        [InfoBox("未勾选特殊逻辑时，将使用默认逻辑，即在应用修改器直接执行数值操作")]
        [LabelText("特殊逻辑")]
        public bool UseSpecLogic;

        [BoxGroup("逻辑配置")]
        [LabelText("逻辑类型")]
        [ShowIf("UseSpecLogic")]
        [ValueDropdown("@GameConfig.DropdownHelper.GetModifierLogicTypes()")]
        public string LogicType;
    }
}
