using System;
using System.Collections.Generic;
using NaughtyAttributes;
using PizzaBar.Scripts.Misc;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using ProjectCore.Spline.Objects;
using UnityEditor;
using UnityEngine;

namespace PizzaBar.Scripts.Managers
{
    public class Table : CachedBehaviour, IService
    {
        Type IService.ServiceType { get; } = typeof(Table);

        [SerializeField] private Spline _spline;
        [SerializeField] private Transform _arrowContainer;
        [SerializeField] private SplineFollow _arrowPrefab;

        [SerializeField] private int _arrowsCount = 56;
        [SerializeField] private float _moveSpeed = 0.1f;
        
        public Spline Spline => _spline;
        public float MoveSpeed => _moveSpeed;

#if UNITY_EDITOR
        [Button]
        public void SetupArrows()
        {
            var oldArrows = new List<GameObject>();
            foreach (Transform o in _arrowContainer)
                oldArrows.Add(o.gameObject);

            for (var i = oldArrows.Count - 1; i >= 0; i--)
                DestroyImmediate(oldArrows[i]);

            for (int i = 0; i < _arrowsCount; i++)
            {
                var arrow = Instantiate(_arrowPrefab, _arrowContainer);
                arrow.Transform.Value.position = Vector3.zero;
                arrow.Percentage = (float) i / _arrowsCount;
                arrow.Initialize(_spline, _moveSpeed);
            }
            
            EditorUtility.SetDirty(this);
        }
#endif
    }
    
    public interface ITableElement
    {
        void OnAddToTable(Spline spline);
        void OnRemoveFromTable();
    }
}