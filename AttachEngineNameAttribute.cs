using System;

namespace AttachManager
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AttachEngineNameAttribute : Attribute
    {
        private string engineName;

        public AttachEngineNameAttribute(string name)
        {
            engineName = name;
        }

        public virtual string EngineName
        {
            get { return engineName; }
        }
    }
}
