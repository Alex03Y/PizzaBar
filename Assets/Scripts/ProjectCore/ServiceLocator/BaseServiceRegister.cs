using System.Collections.Generic;
using System.Linq;
using ProjectCore.Misc;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectCore.ServiceLocator
{
    public abstract class BaseServiceRegister : CachedBehaviour
    {
        public List<ScriptableObject> ScriptableServices = new List<ScriptableObject>();

        public void GetServices(out List<IService> services)
        {
            services = new List<IService>(ScriptableServices.Select(x => (IService) x));
            Services(services);
        }

        protected abstract void Services(List<IService> services);

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var i = 0; i < ScriptableServices.Count; i++)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (!(ScriptableServices[i] is IService))
                {
                    Debug.LogError($"ScriptableObject: {ScriptableServices[i].name} was not implement IService.");
                    ScriptableServices.RemoveAt(i);
                    i--;
                }
            }
        }
#endif
    }
}