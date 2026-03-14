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
            _bulletPool.SetRoot(View.Transform);
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
            bullet.transform.localPosition = new Vector3(Random.Range(-200f, 200f), Random.Range(-200f, 200f), 0);

            Viewer.StartCoroutine(MoveAndDespawn(bullet));

            _signalBus.Invoke(new TestSignal { Message = $"Action executed, click count: {Model.Value}" });
            //if(Model.Value == 10)
                //Close();
        }

        private System.Collections.IEnumerator MoveAndDespawn(TestBullet bullet)
        {
            float elapsed = 0;
            Vector3 startPos = bullet.transform.localPosition;
            Vector3 targetPos = startPos + Vector3.up * 100f;

            while (elapsed < 2f)
            {
                if (bullet == null) yield break;
                bullet.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / 2f);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _bulletPool.Despawn(bullet);
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
