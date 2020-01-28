using System;
using System.Collections.Generic;
using PizzaBar.Scripts.Entities;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;

namespace PizzaBar.Scripts.Managers
{
    public class FoodManager : CachedBehaviour, IService
    {
        Type IService.ServiceType { get; } = typeof(FoodManager);
        
        private readonly Dictionary<int, Food> _foodMap = new Dictionary<int, Food>();

        public void AddFood(Food food)
        {
            if(_foodMap.ContainsKey(food.FoodId))
                throw new IndexOutOfRangeException($"Collection already contains food with id {food.FoodId}");
            
            _foodMap.Add(food.FoodId, food);
        }

        public void RemoveFood(Food food)
        {
            if (_foodMap.ContainsKey(food.FoodId))
                _foodMap.Remove(food.FoodId);
        }

        public Food GetFood(int id)
        {
            _foodMap.TryGetValue(id, out var food);
            return food;
        }
    }
}