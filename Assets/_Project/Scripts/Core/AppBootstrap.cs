using UnityEngine;
using Internal.DI;
using Features.Game.Models;
using Features.Game.Controllers;
using Features.Game.Views;

namespace Core
{
    public class AppBootstrap : MonoBehaviour
    {
        [SerializeField] private CounterView _counterView;

        private CounterController _counterController;

        private void Start()
        {
            // Initialize DI
            var container = ServiceContainer.Instance;

            // Register Services/Models
            var counterModel = new CounterModel { Count = 0 };
            container.Register(counterModel);

            // Initialize Feature (Counter)
            _counterController = new CounterController();
            _counterController.Setup(counterModel, _counterView);
            
            Debug.Log("[AppBootstrap] App initialized!");
        }

        private void OnDestroy()
        {
            _counterController?.Cleanup();
        }
    }
}
