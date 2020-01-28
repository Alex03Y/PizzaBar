using System;
using UnityEngine;

namespace PizzaBar.Scripts.Controllers
{
    public class Trigger2D : MonoBehaviour
    {
        public Action<Collider2D> TriggerEnter, TriggerExit ;
  
        private void OnTriggerEnter2D(Collider2D other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            TriggerExit?.Invoke(other);
        }
    }
}
