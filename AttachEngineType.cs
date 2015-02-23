using System;
using System.Reflection;

namespace AttachToolbar
{
    public enum AttachEngineType
    {
        [AttachEngineNameAttribute("Native")]
        Native = 1,

        [AttachEngineNameAttribute("Managed")]
        Managed = 2,

        [AttachEngineNameAttribute("Managed/Native")]
        Both = 3
    }

    public static class AttachEngineTypeConverter
    {
        public static string GetEngineName(this AttachEngineType type)
        {
            Type atTypeDesc = type.GetType();
            MemberInfo[] info = atTypeDesc.GetMember(type.ToString());
            AttachEngineNameAttribute engineNameAttr = Attribute.GetCustomAttribute(info[0], typeof(AttachEngineNameAttribute))
                as AttachEngineNameAttribute;
            if (engineNameAttr == null)
                throw new ArgumentException();

            return engineNameAttr.EngineName;
        }

        public static AttachEngineType GetAttachType(this string engineName)
        {
            Array enumValues = System.Enum.GetValues(typeof(AttachEngineType));
            foreach (AttachEngineType type in enumValues)
            {
                if (type.GetEngineName() == engineName)
                {
                    return type;
                }
            }

            return AttachEngineType.Native;
        }
    }
}
