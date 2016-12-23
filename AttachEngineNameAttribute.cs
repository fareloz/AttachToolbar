using System;

namespace AttachToolbar
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AttachEngineNameAttribute : Attribute
    {
        public AttachEngineNameAttribute(string name)
        {
            EngineName = name;
        }

        public virtual string EngineName { get; }
    }
}
