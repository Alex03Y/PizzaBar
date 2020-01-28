using System.Reflection;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    public abstract class NativePropertyDrawer
    {
        public abstract void DrawNativeProperty(Object target, PropertyInfo property);
    }
}