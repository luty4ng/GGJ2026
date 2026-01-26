using System;

namespace GameConfig
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigSourceAttribute : Attribute
    {
        public string ConfigName { get; }
        public string IdMemberName { get; }

        public ConfigSourceAttribute(string configName, string idMemberName = "Id")
        {
            ConfigName = configName;
            IdMemberName = idMemberName;
        }
    }
}
