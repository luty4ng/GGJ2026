using System;

namespace GameConfig
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class ConfigOptionAttribute : Attribute
    {
        public string Tag { get; }
        public ConfigOptionAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
