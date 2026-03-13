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

        protected override void OnInitialize()
        {
            // Initializing the controller
            _signalBus.Subscribe<TestSignal>(OnSignalReceived);
            _signalBus.Subscribe<TestLoadMainMenuSignal>(OnLoadMainMenu);
        }

        private void OnLoadMainMenu(TestLoadMainMenuSignal signal)
        {
            Open(new TestModel { Value = 0 });
        }

        protected override void OnOpen()
        {
            View.OnButtonClicked += HandleButtonClick;
            UpdateView();
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

        protected override void OnClose()
        {
            if (View != null)
            {
                View.OnButtonClicked -= HandleButtonClick;
                View.Cleanup();
            }
        }

        protected override void OnDispose()
        {
            _signalBus.Unsubscribe<TestSignal>(OnSignalReceived);
            _signalBus.Unsubscribe<TestLoadMainMenuSignal>(OnLoadMainMenu);
        }
    }
}
