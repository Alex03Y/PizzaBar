using System;

namespace NaughtyAttributes.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class BaseAttribute : Attribute, IAttribute
    {
        public BaseAttribute(Type targetAttributeType)
        {
            TargetAttributeType = targetAttributeType;
        }

        public Type TargetAttributeType { get; }
    }
}