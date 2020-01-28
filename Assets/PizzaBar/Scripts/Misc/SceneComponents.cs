using System;
using ProjectCore.Misc;
using ProjectCore.ServiceLocator;
using UnityEngine;

namespace PizzaBar.Scripts.Misc
{
    public class SceneComponents : CachedBehaviour, IService
    {
        Type IService.ServiceType { get; } = typeof(SceneComponents);

        public Camera MainCamera;
    }
}
