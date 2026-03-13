using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simon.Core.DI
{
    public interface IInstaller
    {
        void InstallBindings();
    }

    public abstract class Installer : IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    public abstract class MonoInstaller : MonoBehaviour, IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    public abstract class ScriptableObjectInstaller : ScriptableObject, IInstaller
    {
        [Inject] protected DiContainer Container { get; set; }
        public abstract void InstallBindings();
    }

    public abstract class PrefabInstaller : MonoInstaller { }
}
