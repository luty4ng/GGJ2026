#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

namespace GameConfig
{
    internal static class DropdownHelper
    {
        internal const string kConfigFolderPath = "Assets/AssetArt/Configs";
        #region ID Options
        internal static IEnumerable<ValueDropdownItem<int>> GetAbilityIdOptions()
        {
            var items = new List<ValueDropdownItem<int>>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AbilityConfig)}", new[] { kConfigFolderPath });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<AbilityConfig>(path);
                if (config == null)
                    continue;

                string label = $"{config.Id} - {config.AbilityName}";
                items.Add(new ValueDropdownItem<int>(label, config.Id));
            }
            return items;
        }

        internal static IEnumerable<ValueDropdownItem<int>> GetFactionIdOptions()
        {
            var items = new List<ValueDropdownItem<int>>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(FactionDefinition)}", new[] { kConfigFolderPath });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<FactionDefinition>(path);
                if (config == null)
                    continue;

                string label = $"{config.Id} - {config.Name}";
                items.Add(new ValueDropdownItem<int>(label, config.Id));
            }
            return items;
        }

        internal static IEnumerable<ValueDropdownItem<int>> GetModifierIdOptions()
        {
            var items = new List<ValueDropdownItem<int>>();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(ModifierConfig)}", new[] { kConfigFolderPath });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<ModifierConfig>(path);
                if (config == null)
                    continue;

                string label = $"{config.Id} - {config.name}";
                items.Add(new ValueDropdownItem<int>(label, config.Id));
            }
            return items;
        }

        #endregion

        #region Type Option
        internal static ValueDropdownList<string> GetStrategyTypeOptions()
        {
            var options = new ValueDropdownList<string>();
            options.Add("<None>", string.Empty);

            var abilityTypes = GameConfigTypeManager.GetTypes()
                .Where(t => IsValidType(t, "TargetSelectStrategy"))
                .OrderBy(t => t.Name);

            foreach (var type in abilityTypes)
            {
                options.Add(type.FullName);
            }
            return options;
        }

        internal static ValueDropdownList<string> GetModifierLogicTypes()
        {
            var options = new ValueDropdownList<string>();
            options.Add("<None>", string.Empty);

            var abilityTypes = GameConfigTypeManager.GetTypes()
                .Where(t => IsValidType(t, "ModifierLogic"))
                .OrderBy(t => t.Name);

            foreach (var type in abilityTypes)
            {
                options.Add(type.FullName);
            }
            return options;
        }

        internal static bool IsValidType(Type type, string tag)
        {
            if (type.IsAbstract || type.IsInterface)
                return false;

            var attrs = type.GetCustomAttributes(typeof(ConfigOptionAttribute), true);
            foreach (var iface in type.GetInterfaces())
            {
                var ifaceAttrs = iface.GetCustomAttributes(typeof(ConfigOptionAttribute), true);
                if (ifaceAttrs.Any(attr => attr is ConfigOptionAttribute configOption && configOption.Tag == tag))
                    return true;
            }
            return false;
        }

        #endregion
    }
}

#endif