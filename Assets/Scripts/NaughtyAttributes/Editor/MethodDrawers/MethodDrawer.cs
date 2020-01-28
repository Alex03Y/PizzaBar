using System.Reflection;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    public abstract class MethodDrawer
    {
        public abstract void DrawMethod(Object target, MethodInfo methodInfo);
    }
}