using Simon.Core.MVC;
using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestController : MvcController<ITestView, TestModel>
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private TestItemPool _itemPool;
        [Inject] private TestBulletPool _bulletPool;
        [Inject] private IViewSource<ITestView> _viewSource;

        protected override void OnInitialize()
        {
            // 1. Setup Model
            Setup(new TestModel());

            // 2. Spawn View using IViewSource (if not already set via manual DI)
            if (View == null)
            {
                var view = _viewSource.Spawn(null); // Parent could be specified here
                Setup(view, Model);
            }

            View.OnButtonClicked += HandleButtonClick;
            UpdateView();
            
            _signalBus.Subscribe<TestSignal>(OnSignalReceived);
        }

        private void UpdateView()
        {
            View.SetText($"Clicks: {Model.Value}");
        }

        private void HandleButtonClick()
        {
            Model.Value++;
            UpdateView();
            
            // 1. Demonstrate Class Pool
            var item = _itemPool.Spawn();
            _itemPool.Despawn(item);

            // 2. Demonstrate Object Pool
            var bullet = _bulletPool.Spawn();
            _bulletPool.Despawn(bullet);

            _signalBus.Invoke(new TestSignal { Message = $"Action executed, click count: {Model.Value}" });
        }

        private void OnSignalReceived(TestSignal signal)
        {
            Debug.Log($"[TestController] Signal Received: {signal.Message}");
        }

        protected override void OnDispose()
        {
            _signalBus.Unsubscribe<TestSignal>(OnSignalReceived);
            if (View != null)
            {
                View.OnButtonClicked -= HandleButtonClick;
                View.Cleanup();
            }
        }
    }
}
