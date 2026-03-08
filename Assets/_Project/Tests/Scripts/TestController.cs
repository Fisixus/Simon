using Simon.Core.MVC;
using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestController : MvcController<ITestView, TestModel>
    {
        [Inject] private SignalBus _signalBus;

        public override void Initialize()
        {
            // Models are just data containers, so we create it here
            Setup(new TestModel { Value = 100 });

            View.OnButtonClicked += HandleButtonClick;
            View.SetText($"Initial value from model: {Model.Value}");
            
            _signalBus.Subscribe<TestSignal>(OnSignalReceived);
        }

        private void HandleButtonClick()
        {
            Model.Value++;
            View.SetText($"Updated value: {Model.Value}");
            _signalBus.Invoke(new TestSignal { Message = $"Value incremented to {Model.Value}" });
        }

        private void OnSignalReceived(TestSignal signal)
        {
            Debug.Log($"[TestController] Signal Received: {signal.Message}");
        }

        public override void Dispose()
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
