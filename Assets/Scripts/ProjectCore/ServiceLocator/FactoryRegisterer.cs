using ProjectCore.Factory;
using UnityEngine;

namespace ProjectCore.ServiceLocator
{
    public class FactoryRegisterer : MonoBehaviour
    {
        private void Awake()
        {
            var factories = GetComponentsInChildren<IFactory>();
            foreach (var factory in factories)
                ServiceLocator.Register(factory);
        }
    }
}
