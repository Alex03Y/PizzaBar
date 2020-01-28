using PizzaBar.Scripts.Controllers;
using PizzaBar.Scripts.Factories;
using ProjectCore.Factory;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using ProjectCore.Timer.Source;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PizzaBar.Scripts.Entities
{
    public class Cook : CachedBehaviour
    {
        [SerializeField] private FoodType[] _foodTypesForSpawn;
        [SerializeField] private float _spawnDelay;
        [SerializeField] private Transform _foodSpawnPoint;
        [SerializeField] private Trigger2D _trigger;
        
        private IFactory<Food, FoodCreationArgs> _foodFactory;
        private int _countOfPizzaInFront;
        
        private void Awake()
        {
            _foodFactory = ServiceLocator.Resolve<IFactory<Food, FoodCreationArgs>>();
            
            _trigger.TriggerEnter += nothing => _countOfPizzaInFront++;
            _trigger.TriggerExit += nothing => _countOfPizzaInFront--;
            SpawnFood();
            
        }

        private void SpawnFood()
        {
            if (_countOfPizzaInFront > 0)
            {
                Timer.Register(0.2f, SpawnFood);
                return;
            }
            
            var foodTypeToSpawn = _foodTypesForSpawn[Random.Range(0, _foodTypesForSpawn.Length - 1)];
            var foodCreationArgs = new FoodCreationArgs(foodTypeToSpawn, _foodSpawnPoint.position);
            _foodFactory.Create(foodCreationArgs);
            
            Timer.Register(_spawnDelay, SpawnFood);
        }
    }
}