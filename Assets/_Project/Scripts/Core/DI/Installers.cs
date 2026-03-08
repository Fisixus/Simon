using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    internal interface IInstaller
    {
        void InstallBindings();
    }

    internal abstract class Installer : IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    internal abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    internal abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    internal abstract class PrefabInstaller : MonoInstaller { }
}
