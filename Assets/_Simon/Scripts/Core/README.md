# Simon Core

A lightweight, high-performance **Dependency Injection (DI)** and **MVC** framework for Unity.

## How to Install in Other Projects

1. Open your other Unity project.
2. Open the **Package Manager** (`Window > Package Manager`).
3. Click the `+` button in the top left and select **"Add package from disk..."**.
4. Navigate to this folder (`Assets/_Simon/Scripts/Core`) and select the `package.json` file.

## Key Features

- **Hierarchical DI:** Support for Project, Scene, and GameObject level containers.
- **Factory & Pooling:** Integrated object factories and pooling systems.
- **MVC Framework:** Decoupled Controller, Model, and View architecture with automated UI root parenting.
- **Signal Bus:** Lightweight event system for cross-controller communication.

## Quick Start

### 1. Create a Project Context
Add a `ProjectContext` to your initial scene (or use the one in Resources) and assign a `MonoInstaller`.

### 2. Create an Installer
```csharp
public class MyInstaller : MonoInstaller {
    public override void InstallBindings() {
        Container.Bind<MyService>().AsSingle();
    }
}
```

### 3. Create a Controller
```csharp
public class MyController : MvcController<IMyView, MyModel> {
    [Inject] private MyService _service;
    
    protected override void OnOpen() {
        // View is automatically spawned and parented to the Viewer/Context
    }
}
```
