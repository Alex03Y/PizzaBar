using System;
using DG.Tweening;
using NaughtyAttributes;
using PizzaBar.Scripts.Factories;
using PizzaBar.Scripts.Misc;
using ProjectCore.Misc;
using ProjectCore.PoolManager;
using UnityEngine;

namespace PizzaBar.Scripts.Entities
{
    public class Food : CachedBehaviour, IPoolObject
    {
        [SerializeField, BoxGroup("Main")] private FoodType _foodType;
        [SerializeField, BoxGroup("Main")] private Collider2D _collider;
        [SerializeField, BoxGroup("Main")] private Transform _partsRoot;
        [SerializeField, BoxGroup("Main")] private SplineFollow _splineFollow;
        [SerializeField, BoxGroup("Main")] private int _costOfPart;
        [SerializeField, BoxGroup("Main")] private int _amountOfExpirience;
        
        [SerializeField, BoxGroup("VFX")] private ParticleSystem _spawnParticles;

        private PoolObject _poolObject;
        private Transform[] _partsTransforms;
        private Vector3[] _partsLocalPositions;
        private Vector3 _defaultScale;
        
        public FoodType FoodType => _foodType;
        public int CostOfPart => _costOfPart;
        public int AmountOfExpirience => _amountOfExpirience;

        public int FoodId => _collider.GetInstanceID();

        public Transform[] PartsTransforms => _partsTransforms;

        #region pool object
        public void PostAwake(PoolObject poolObject)
        {
            _poolObject = poolObject;
            poolObject.Register(typeof(Food), this);
            poolObject.Register(typeof(SplineFollow), _splineFollow);
        }

        public void OnReuseObject(PoolObject poolObject)
        {
            OnSpawn();
        }

        public void OnDisposeObject(PoolObject poolObject)
        {
            _partsRoot.localScale = Vector3.one;
            for (var i = 0; i < _partsTransforms.Length; i++)
            {
                _partsTransforms[i].localScale = Vector3.one;
                _partsTransforms[i].localPosition = _partsLocalPositions[i];
            }
        }
        #endregion

        private void Awake()
        {
            if (_collider == null)
                gameObject.AutoResolveComponent(out _collider);

            var parts = _partsRoot.GetComponentsInChildren<SpriteRenderer>();
            _partsTransforms = new Transform[parts.Length];
            _partsLocalPositions = new Vector3[parts.Length];
            
            for (var i = 0; i < _partsTransforms.Length; i++)
            {
                _partsTransforms[i] = parts[i].transform;
                _partsLocalPositions[i] = _partsTransforms[i].localPosition;
            }

            _defaultScale = Transform.Value.localScale;
        }
        
        public void GrabFromTable()
        {
            _splineFollow.SetInitializeStatus(false);
        }

        public void Slice(Action onSliceCompleted)
        {
            var sequence = DOTween.Sequence();
            var scaleRoot = _partsRoot.DOScale(Vector3.one * 1.1f, 0.35f);
            sequence.Append(scaleRoot);

            foreach (var partsTransform in _partsTransforms)
            {
                var scaleChild = partsTransform.DOScale(Vector3.one / 1.1f, 0.15f);
                sequence.Append(scaleChild);
            }

            sequence.OnComplete(() => onSliceCompleted?.Invoke());
            sequence.Play();
        }

        private void OnSpawn()
        {
            if (_defaultScale == Vector3.zero)
                _defaultScale = Transform.Value.localScale;
            
            Transform.Value.localScale = Vector3.zero;
            Transform.Value.DOScale(_defaultScale, 0.35f);
            _spawnParticles.Play(true);
        }

        public void ReturnToPool()
        {
            _poolObject.Destroy();
        }
    }
}