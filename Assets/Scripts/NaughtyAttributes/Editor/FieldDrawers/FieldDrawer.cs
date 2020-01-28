using System.Reflection;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    public abstract class FieldDrawer
    {
        public abstract void DrawField(Object target, FieldInfo field);
    }
}