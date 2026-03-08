using Simon.Core.MVC;
using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestController : MvcController<TestView, TestModel>
    {
        [Inject] private SignalBus _signalBus;

        public override void Initialize()
        {
            View.OnButtonClicked += HandleButtonClick;
            View.SetText($"Initial value from model: {Model.Value}");
            _signalBus.Subscribe<TestSignal>(OnSignalReceived);
        }

        private void HandleButtonClick()
        {
            Model.Value++;
            View.SetText($"Updated value: {Model.Value}");
            _signalBus.Fire(new TestSignal { Message = $"Value incremented to {Model.Value}" });
        }

        private void OnSignalReceived(TestSignal signal)
        {
            Debug.Log($"[TestController] Signal Received: {signal.Message}");
        }

        public override void Dispose()
        {
            View.OnButtonClicked -= HandleButtonClick;
            View.Cleanup();
        }
    }
}
