using System;
using NaughtyAttributes;
using PizzaBar.Scripts.GUI.Models;
using PizzaBar.Scripts.GUI.Views;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using UnityEngine;

namespace PizzaBar.Scripts.Managers
{
    public class GameManager : CachedBehaviour, IService
    {
        Type IService.ServiceType { get; } = typeof(GameManager);

        [SerializeField] private bool _enableEdit = false;
        [SerializeField] private HeaderView _headerView;
        [SerializeField, EnableIf("_enableEdit")] private int _money;
        [SerializeField, EnableIf("_enableEdit")] private int _experience;

        private HeaderMenuModel _headerMenuModel;

        private void Awake()
        {
            _headerMenuModel = ServiceLocator.Resolve<HeaderMenuModel>();
        }

        public void AddMoney(int moneyAmount)
        {
            if(moneyAmount < 0)
                throw new ArgumentException("Argument should be greater then zero.");
            
            _money += moneyAmount;
            _headerMenuModel.Money.Value = _money;
        }

        public void AddExperience(int experienceAmount)
        {
            if(experienceAmount < 0)
                throw new ArgumentException("Argument should be greater then zero.");

            _experience += experienceAmount;
            _headerMenuModel.Experience.Value = _experience;
        }
    }
}
