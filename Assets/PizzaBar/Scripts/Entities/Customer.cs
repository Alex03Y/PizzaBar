using DG.Tweening;
using Klak.Motion;
using PizzaBar.Scripts.Controllers;
using PizzaBar.Scripts.Managers;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using UnityEngine;
using Timer = ProjectCore.Timer.Source.Timer;

namespace PizzaBar.Scripts.Entities
{
    public class Customer : CachedBehaviour
    {
//        [SerializeField] private int ServingsFoodToEat = 3;
//        [SerializeField] private float TimeToEatFood = 4f;
        [SerializeField] private Trigger2D _trigger2D;
        [SerializeField] private Transform _positionToEat;
        [SerializeField] private float _durationToMoveFood = 0.35f;
        [SerializeField] private BrownianMotion _rightHandAnimation;
        [SerializeField] private BrownianMotion _leftHandAnimation;

//        private bool _coolDownEating = true;
        private FoodManager _foodManager;
        private GameManager _gameManager;
        private Transform[] _partsPizza;
        private Food _food;

        private void Awake()
        {
            _foodManager = ServiceLocator.Resolve<FoodManager>();
            _gameManager = ServiceLocator.Resolve<GameManager>();
            _trigger2D.TriggerEnter += GrabFoodFromTable;
            AnimationHandTumbler(false);
        }

        private void GrabFoodFromTable(Collider2D collider)
        {
//            if (ServingsFoodToEat == 0) return;
           
            if (_food != null) return;
//            ServingsFoodToEat--;
            
            _food = _foodManager.GetFood(collider.GetInstanceID());
            _food.GrabFromTable();
            AnimationHandTumbler(true);
            _food.Transform.Value.DOMove(_positionToEat.position, _durationToMoveFood)
                .OnComplete(() => _food.Slice(() => EatingPartsPizza(_partsPizza.Length - 1)));
            _partsPizza = _food.PartsTransforms;
        }

        private void EatingPartsPizza(int count)
        {
            if (count < 0)
            {
                _gameManager.AddExperience(_food.AmountOfExpirience);
                AnimationHandTumbler(false);
                _food.ReturnToPool();
                _food = null;
                return;
            }
            
            _partsPizza[count].DOMove(Transform.Value.position, _durationToMoveFood);
            _partsPizza[count].DOScale(Vector3.zero, _durationToMoveFood / 2f).OnComplete(() =>
            {
                _gameManager.AddMoney(_food.CostOfPart);
            });
            Timer.Register(_durationToMoveFood, () => EatingPartsPizza(count - 1));
        }

        private void AnimationHandTumbler(bool tumbler)
        {
            _rightHandAnimation.enableRotationNoise = tumbler;
            _leftHandAnimation.enableRotationNoise = tumbler;
        }
    }
}