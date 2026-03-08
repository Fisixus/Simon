namespace Simon.Core.DI
{
    internal interface IInstaller
    {
        void InstallBindings();
    }

    internal abstract class MonoInstaller : UnityEngine.MonoBehaviour, IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }
}
