using System;
using System.Collections.Generic;
using ProjectCore.Misc;
using UnityEngine;

namespace ProjectCore.ServiceLocator
{
    [RequireComponent(typeof(ServiceRegister))]
    public class ServiceLocator : CachedBehaviour
    {
        private static readonly Dictionary<Type, IService> ServiceMap = new Dictionary<Type, IService>();

        private void Awake()
        {
            var serviceRegisterer = GetComponent<BaseServiceRegister>();
            if (serviceRegisterer.IsNotNull())
            {
                serviceRegisterer.GetServices(out var services);
                services.ForEach(Register);
            }
        }

        public static void Register(IService service)
        {
            if (ServiceMap.ContainsKey(service.ServiceType))
                throw new Exception($"[ServiceLocator] Service {service.ServiceType.Name} already registered.");

            ServiceMap.Add(service.ServiceType, service);
        }

        public static TRegister Resolve<TRegister>() where TRegister : class
        {
            var serviceType = typeof(TRegister);

            if (ServiceMap.ContainsKey(serviceType)) return (TRegister) ServiceMap[serviceType];
            throw new Exception($"[ServiceLocator] Service {serviceType.FullName} was not being register.");
        }

        private void OnDestroy()
        {
            ServiceMap.Clear();
        }
    }
}