using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectCore.PoolManager
{
    [Serializable]
    public class PoolObject
    {
        public Transform Transform { get; }
        public GameObject GameObject { get; }
        public int InstanceId { get; private set; }
        public IPoolObject[] PoolObjectScripts { get; }
        public Transform Parent { get; }
        public bool IsInsidePool { get; internal set; }

        public event Action OnDestroyed;

        private readonly Dictionary<Type, List<Component>> _componentsMap = new Dictionary<Type, List<Component>>();

        public PoolObject(GameObject instance, Transform parent)
        {
            GameObject = instance;
            InstanceId = instance.GetInstanceID();
            Transform = instance.transform;
            PoolObjectScripts = instance.GetComponentsInChildren<IPoolObject>(true);

            Parent = parent;

            foreach (var poolObjectScript in PoolObjectScripts)
                poolObjectScript.PostAwake(this);
        }

        /// <summary>
        /// Save component into map.
        /// </summary>
        /// <param name="saveType">Type was been assigned to component.</param>
        /// <param name="component">Component for caching.</param>
        public void Register(Type saveType, Component component)
        {
            if (_componentsMap.TryGetValue(saveType, out var components)) components.Add(component);
            else _componentsMap.Add(saveType, new List<Component> {component});
        }

        /// <summary>
        /// Return component from cache.
        /// </summary>
        /// <typeparam name="TSave">Type was been assigned to component.</typeparam>
        /// <returns>Cached component or null.</returns>
        public TSave Resolve<TSave>() where TSave : Component
        {
            var saveType = typeof(TSave);

            if (_componentsMap.TryGetValue(saveType, out var components))
                return (TSave) components[0];

            return null;
        }

        /// <summary>
        /// Works similar to Resolve() method.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TSave"></typeparam>
        /// <returns></returns>
        public bool TryResolve<TSave>(out TSave instance) where TSave : Component
        {
            var saveType = typeof(TSave);

            if (_componentsMap.TryGetValue(saveType, out var components))
            {
                instance = (TSave) components[0];
                return true;
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// Return array of similar cached components.
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="TSave"></typeparam>
        /// <returns></returns>
        public bool TryResolveMany<TSave>(out List<TSave> instance) where TSave : Component
        {
            var saveType = typeof(TSave);

            if (_componentsMap.TryGetValue(saveType, out var components))
            {
                instance = components.Select(x => (TSave) x).ToList();
                return true;
            }

            instance = null;
            return false;
        }

        /// <summary>
        /// Return object back into pool.
        /// </summary>
        public void Destroy() // todo: not safety, should cleanup fields and save copy of object
        {
            GameObject.SetActive(false);
            Transform.position = Vector3.zero;
            Transform.rotation = Quaternion.identity;
            Transform.parent = Parent;
            IsInsidePool = true;

            foreach (var poolObjectScript in PoolObjectScripts)
                poolObjectScript.OnDisposeObject(this);
            
            OnDestroyed?.Invoke();
            OnDestroyed = delegate { };
        }
    }
}