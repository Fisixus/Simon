using Simon.Core.DI;
using UnityEngine;

namespace Simon.Tests
{
    internal class TestViewFactory : ObjectFactory<TestView>, IViewSource<ITestView>
    {
        public ITestView Spawn(Transform parent)
        {
            var view = base.Spawn(parent);
            view.Initialize();
            return view;
        }

        public void Despawn(ITestView item)
        {
            if (item is MonoBehaviour mono)
            {
                if(item != null)
                    UnityEngine.Object.Destroy(mono.gameObject);
            }
        }

    }
}
