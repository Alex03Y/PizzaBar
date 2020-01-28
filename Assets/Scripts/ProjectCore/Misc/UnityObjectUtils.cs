using UnityEngine;

namespace ProjectCore.Misc
{
    public static class UnityObjectUtils
    {
        public static bool IsNull(this UnityEngine.Object o)
        {
            return ReferenceEquals(o, null);
        }

        public static bool IsNotNull(this UnityEngine.Object o)
        {
            return !ReferenceEquals(o, null);
        }

        public static bool RefEquals(this UnityEngine.Object o, UnityEngine.Object obj)
        {
            return ReferenceEquals(o, obj);
        }

        public static bool InstanceIdEquals(this CachedBehaviour o, CachedBehaviour obj)
        {
            return o.InstanceId == obj.InstanceId;
        }

        public static void AutoResolveComponent<T>(this GameObject container, out T component) where T : Component
        {
            Debug.LogError("Component has not been assigned in inspector, try auto-resolve.");
            component = container.GetComponent<T>();
            if (component == null) component = container.GetComponentInChildren<T>();
        }
    }
}