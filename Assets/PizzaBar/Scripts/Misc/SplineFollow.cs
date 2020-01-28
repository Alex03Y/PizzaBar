using ProjectCore.Misc;
using ProjectCore.PoolManager;
using ProjectCore.Spline.Objects;
using UnityEngine;

namespace PizzaBar.Scripts.Misc
{
    public class SplineFollow : CachedBehaviour, IPoolObject
    {
        [SerializeField] private Spline _spline ;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _percentage;
        [SerializeField] private bool _isInitialized;

        public float Percentage
        {
            set => _percentage = value;
        }

        public void Initialize(Spline spline, float speed)
        {
            _spline = spline;
            _moveSpeed = speed;
            _isInitialized = true;
        }

        private void Update()
        {
            if(!_isInitialized) return;

            _percentage += Time.deltaTime * _moveSpeed;
            if (_percentage >= 1f)
            {
                _percentage = 0f;
                return;
            }

            Quaternion rotation;
            if(_spline.direction == SplineDirection.Forward)
                rotation = Quaternion.LookRotation(-Vector3.forward, _spline.GetDirection(_percentage));
            else rotation = Quaternion.LookRotation(Vector3.forward, _spline.GetDirection(_percentage));

            Transform.Value.rotation = rotation;
            Transform.Value.position = _spline.GetPosition(_percentage);
        }

        public void SetInitializeStatus(bool status)
        {
            _isInitialized = status;
        }
        
        public void PostAwake(PoolObject poolObject)
        {
            // do nothing
        }

        public void OnReuseObject(PoolObject poolObject)
        {
            // do nothing
        }

        public void OnDisposeObject(PoolObject poolObject)
        {
            _isInitialized = false;
        }
    }
}
