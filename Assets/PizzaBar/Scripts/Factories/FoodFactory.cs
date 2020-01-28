using System;
using System.Collections.Generic;
using DG.Tweening;
using PizzaBar.Scripts.Entities;
using PizzaBar.Scripts.Managers;
using PizzaBar.Scripts.Misc;
using ProjectCore.Factory;
using ProjectCore.Misc;
using ProjectCore.PoolManager;
using ProjectCore.ServiceLocator;
using UnityEngine;

namespace PizzaBar.Scripts.Factories
{
    public class FoodFactory : MonoBehaviour, IFactory<Food, FoodCreationArgs>
    {
        Type IService.ServiceType { get; } = typeof(IFactory<Food, FoodCreationArgs>);
        
        public List<Food> FoodPrefabs = new List<Food>();

        private readonly Dictionary<FoodType, GameObject> _prefabMap = new Dictionary<FoodType, GameObject>();
        private PoolManager _poolManager;
        private FoodManager _foodManager;
        private Table _table;
        
        private void Awake()
        {
            _poolManager = ServiceLocator.Resolve<PoolManager>();
            _foodManager = ServiceLocator.Resolve<FoodManager>();
            _table = ServiceLocator.Resolve<Table>();
            
            FoodPrefabs.ForEach(x =>
            {
                _prefabMap.Add(x.FoodType, x.gameObject);
                _poolManager.CreatePool(x.gameObject, 20, 5);
            });
        }

        public Food Create(FoodCreationArgs args)
        {
            var poolObject = _poolManager.InstantiateFromPool(_prefabMap[args.FoodType], args.Position);
            
            RegisterFood(poolObject);
            SnapFootToSpline(poolObject);
            
            return poolObject.Resolve<Food>();
        }

        private void SnapFootToSpline(PoolObject poolObject)
        {
            var spline = _table.Spline;
            var splineClosestPercentage = spline.ClosestPoint(poolObject.Transform.position);
            var splineClosestPoint = spline.GetPosition(splineClosestPercentage);

            poolObject.Transform.DOMove(splineClosestPoint, 0.35f).OnComplete(() =>
            {
                var splineFollow = poolObject.Resolve<SplineFollow>();
                splineFollow.Percentage = splineClosestPercentage;
                splineFollow.Initialize(spline, _table.MoveSpeed);
            });
        }

        private void RegisterFood(PoolObject poolObject)
        {
            var food = poolObject.Resolve<Food>();

            _foodManager.AddFood(food);
            poolObject.OnDestroyed += () => _foodManager.RemoveFood(food);
        }
    }
    
    public enum FoodType
    {
        // todo: add new types of food here
        // be careful with enum serialization in unity inspector
            
        Pizza = 0
    }

    public class FoodCreationArgs : IFactoryArgs
    {
        public readonly FoodType FoodType;
        public readonly Vector3 Position;

        public FoodCreationArgs(FoodType foodType, Vector3 position)
        {
            FoodType = foodType;
            Position = position;
        }
    }
}