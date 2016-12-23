using System;

namespace AttachToolbar
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EngineNameAttribute : Attribute
    {
        public EngineNameAttribute(string name)
        {
            EngineName = name;
        }

        public virtual string EngineName { get; }
    }
}
