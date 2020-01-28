using System;
using ProjectCore.Observer;
using ProjectCore.ServiceLocator;

namespace PizzaBar.Scripts.GUI.Models
{
    public class HeaderMenuModel : IService
    {
        Type IService.ServiceType { get; } = typeof(HeaderMenuModel);
        
        public readonly ReactiveProperty<int> Money = new ReactiveProperty<int>(0);
        public readonly ReactiveProperty<int> Experience = new ReactiveProperty<int>(0);
    }
}