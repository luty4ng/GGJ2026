using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;

namespace GameConfig
{
    public static class GameConfigTypeManager
    {
        private static List<Assembly> m_assemblies = new List<Assembly>();
        private static Dictionary<string, Type> m_typeMap = new Dictionary<string, Type>();

        public static Type GetType(string typeName)
        {
            if (m_typeMap.ContainsKey(typeName))
                return m_typeMap[typeName];
            return null;
        }

        public static IEnumerable<Type> GetTypes()
        {
            MakeSureCache();
            return m_typeMap.Values;
        }

        private static void MakeSureCache()
        {
            if (m_assemblies.Count == 0)
                m_assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            if (m_typeMap.Count == 0)
            {
                foreach (var assembly in m_assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        m_typeMap[type.Name] = type;
                    }
                }
            }
        }
    }
}