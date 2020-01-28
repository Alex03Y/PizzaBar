using System.Collections.Generic;
using PizzaBar.Scripts.GUI.Models;
using ProjectCore.ServiceLocator;

namespace PizzaBar.Scripts.GlobalObjects
{
    public class ModelsRegisterer : BaseServiceRegister
    {
        protected override void Services(List<IService> services)
        {
            services.Add(new HeaderMenuModel());
        }
    }
}
