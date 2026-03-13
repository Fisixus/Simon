using Simon.Core.Events;
using UnityEngine;

namespace Simon.Core.DI
{
    internal class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SignalBus>().FromInstance(new SignalBus()).AsSingle();
        }
    }
}
