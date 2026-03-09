using Simon.Core.MVC;

namespace Simon.Tests
{
    internal class TestModel
    {
        public ReactiveProperty<int> Value = new ReactiveProperty<int>(0);
    }
}
