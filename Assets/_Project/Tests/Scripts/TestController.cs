using Simon.Core.MVC;
using Simon.Core.DI;
using Simon.Core.Events;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestController : MvcController<TestView, TestModel>
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private TestView _view;

        public override void Initialize()
        {
            // Set up manually for now, or use a Model binding if we wanted it automated
            // But we're keeping Models as data containers.
            var testModel = new TestModel { Value = 100 };
            Setup(_view, testModel);

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
