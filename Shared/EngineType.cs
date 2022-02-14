using System;
using System.Reflection;

namespace AttachToolbar
{
    public enum EngineType
    {
        [EngineName("Native")]
        Native = 1,

        [EngineName("Managed")]
        Managed = 2,

        [EngineName("Managed/Native")]
        Both = 3
    }

    public static class AttachEngineTypeConverter
    {
        public static string GetEngineName(this EngineType type)
        {
            Type atTypeDesc = type.GetType();
            MemberInfo[] info = atTypeDesc.GetMember(type.ToString());
            EngineNameAttribute engineNameAttr = Attribute.GetCustomAttribute(info[0], typeof(EngineNameAttribute))
                as EngineNameAttribute;
            if (engineNameAttr == null)
                throw new NullReferenceException("Cannot get engine name");

            return engineNameAttr.EngineName;
        }

        public static EngineType GetAttachType(this string engineName)
        {
            Array enumValues = Enum.GetValues(typeof(EngineType));
            foreach (EngineType type in enumValues)
            {
                if (type.GetEngineName() == engineName)
                {
                    return type;
                }
            }

            return EngineType.Native;
        }
    }
}
