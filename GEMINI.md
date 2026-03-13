# Simon Project Overview

Simon is a Unity-based 2D project utilizing a custom-built architecture for Dependency Injection (DI) and Model-View-Controller (MVC) patterns. It leverages the Universal Render Pipeline (URP) and the new Input System.

## Project Structure

- **Assets/_Project/Scripts/Core**: Contains the architectural foundation.
    - **DI**: Dependency Injection container, contexts (`ProjectContext`, `Context`, `GameObjectContext`), and installers.
    - **MVC**: Base classes for Views and Controllers.
    - **Events**: `SignalBus` for decoupled event-driven communication.
- **Assembly Definitions**:
    - `Simon.Core.asmdef`: Core framework assembly.
    - `Simon.Tests.asmdef`: Testing assembly with access to core internals via `InternalsVisibleTo`.
- **Assets/_Project/Tests**: Contains architectural tests and example usage patterns.

## Architecture & Frameworks

### Dependency Injection (Simon.Core.DI)
The project uses a custom IoC container that supports:
- **Contexts**: Hierarchical lifetime management via `ProjectContext` (singleton), `Context` (scene-level), and `GameObjectContext`.
- **Injection**: Support for Constructor, Field, Property, and Method injection using the `[Inject]` attribute.
- **Caching**: Reflection data (constructors, fields, properties, methods) is cached for performance.
- **Circular Dependency Detection**: Throws an exception if a circular dependency is detected.
- **Installers**: Bindings are configured in `MonoInstaller`, `ScriptableObjectInstaller`, or `PrefabInstaller`.

### MVC Pattern (Simon.Core.MVC)
- **Model**: Data classes holding state.
- **View**: `MonoBehaviour` classes inheriting from `MvcView` and implementing a view interface.
- **Controller**: C# classes inheriting from `MvcController<TViewInterface, TModel>`. They use `OnInitialize()` and ` OnDispose()` lifecycle hooks.

### Event System (Simon.Core.Events)
A `SignalBus` is used for global or context-specific communication. Signals are defined as simple classes or structs.

### Pooling & Factories
- **Pools**: Support for Class pools and Object pools (Unity Prefabs) via `ClassPool<T>` and `ObjectPool<T>`.
- **Pre-warming**: Pools support `MinimumCount` for pre-instantiating objects upon initialization.
- **Factories**: Used for spawning views and complex objects while maintaining DI integrity.

## Building and Running

### Requirements
- **Unity Version**: 2022.3 or newer.
- **Render Pipeline**: Universal Render Pipeline (URP).

### Key Commands
- **Building**: Use the Unity Build Settings.
- **Running**: Press "Play" in the Unity Editor.
- **Testing**: Open the Unity Test Runner (`Window > General > Test Runner`).

## Development Conventions

### Coding Style
- **Namespaces**: Use `Simon` as the root namespace.
- **Naming**: `PascalCase` for classes/methods, `_camelCase` for private fields, `IPrefixed` for interfaces.
- **Lifecycle**: Use `OnInitialize()` and `OnDispose()` in controllers for setup and cleanup.
- **Quality Assurance**: ALWAYS check for errors (compilation, logical) after making any change to the codebase.

## TODO / Known Issues
- [ ] Implement automated build pipeline scripts.
- [ ] Expand documentation on specific game features.
- [ ] Implement pool shrink logic if memory becomes an issue.
