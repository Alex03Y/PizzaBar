using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProjectCore.PoolManager
{
    public class PoolQueue
    {
        private readonly Queue<PoolObject> _poolObjects = new Queue<PoolObject>();

        private readonly GameObject _prefab;
        private readonly Transform _poolParent;

        private readonly int _increaseSizeBy;
        private bool _flexibleSize => _increaseSizeBy > 0;

        public PoolQueue(GameObject prefab, int startSize, int increaseSizeBy, Transform poolRoot)
        {
            _prefab = prefab;
            _increaseSizeBy = increaseSizeBy;

            _poolParent = new GameObject($"Pool_{prefab.name}").transform;
            _poolParent.parent = poolRoot;

            for (var i = 0; i < startSize; i++) _poolObjects.Enqueue(CreatePoolObject(_prefab));
        }

        public PoolObject GetPoolObject()
        {
            var poolObject = _poolObjects.Dequeue();
            _poolObjects.Enqueue(poolObject);

            if (poolObject.GameObject.activeSelf && !_flexibleSize)
            {
                poolObject.Destroy();
            }
            else if (poolObject.GameObject.activeSelf && _flexibleSize)
            {
                poolObject = CreatePoolObject(_prefab);
                _poolObjects.Enqueue(poolObject);

                for (int i = 0; i < _increaseSizeBy - 1; i++)
                    _poolObjects.Enqueue(CreatePoolObject(_prefab));
            }

            poolObject.GameObject.SetActive(true);
            poolObject.IsInsidePool = false;
            
            foreach (var iPoolObject in poolObject.PoolObjectScripts)
                iPoolObject.OnReuseObject(poolObject);

            return poolObject;
        }

        public void DisposePool(float timeToDestroyPool)
        {
            if (Math.Abs(timeToDestroyPool) < 0.0001f)
            {
                while (_poolObjects.Count != 0)
                    Object.Destroy(_poolObjects.Dequeue().GameObject);
            }
            else
            {
                var poolSize = _poolObjects.Count;
                var timePerObject = timeToDestroyPool / poolSize;
                DisposeRecursively(timePerObject);
            }
        }

        private void DisposeRecursively(float timePerObject)
        {
            if (_poolObjects.Count == 0) return;
            Timer.Source.Timer.Register(timePerObject, () =>
            {
                Object.Destroy(_poolObjects.Dequeue().GameObject);
                DisposeRecursively(timePerObject);
            });
        }

        private PoolObject CreatePoolObject(GameObject prefab)
        {
            var instance = Object.Instantiate(prefab, _poolParent);
            instance.SetActive(false);

            return new PoolObject(instance, _poolParent);
        }
    }
}