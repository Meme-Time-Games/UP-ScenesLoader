# ScenesLoader

Loads and unloads additive scenes described by `SceneDataSO` assets. It shows the loading screen,
removes the scenes that have to be closed, opens the scenes of the `SceneData`, runs the command
queue that each opened scene registers and notifies when all the scenes are loaded.

## How it works

`SceneLoaderInstaller` builds the `SceneLoader` and registers it as `ISceneLoader` in the
`ServiceLocator`. The loader survives the scene changes and is only built once, so installing it
again keeps the loader that is already alive.

```
SceneLoader (Domain)          the loading state machine, plain C#
ISceneOperations (Domain)     load, unload and activate scenes
SceneManagerOperations        the ISceneOperations that talks to the Unity SceneManager
IDelayProvider (Domain)       waits seconds or a frame
MonoDelayProvider             the IDelayProvider that waits with coroutines
```

Every scene that has to be initialized before the next scene is opened installs a
`SceneLoadedNotifierInstaller`, which gives its `ICommandQueue` to the loader. A scene without a
notifier does not stop the loading.

## Usage

```csharp
[Inject] private ISceneDataLoader _sceneDataLoader;

_sceneDataLoader.Load();                    // loads its SceneData
_sceneDataLoader.LoadKeepingOpenScenes();   // loads it without removing the open scenes
```

```csharp
ISceneLoader sceneLoader = ServiceLocatorInstance.Instance.Get<ISceneLoader>();

sceneLoader.LoadScene(sceneData);
sceneLoader.ReloadCurrentScene();
sceneLoader.RemoveScene(sceneData);
sceneLoader.OnAllScenesLoaded += Initialize;
```

## Migration to 2.0.0

| 1.x                                       | 2.0.0                                       |
|-------------------------------------------|---------------------------------------------|
| `ISceneLoader.OnAllScenesAreLoaded`       | `ISceneLoader.OnAllScenesLoaded`            |
| `ISceneLoader.LoadScene(sceneData, true)` | `ISceneLoader.LoadSceneKeepingOpenScenes(sceneData)` |
| `ISceneDataLoader.Load(true)`             | `ISceneDataLoader.LoadKeepingOpenScenes()`  |
| `SceneDataLoader.IsThiSceneDataOpened()`  | `SceneDataLoader.IsThisSceneDataOpened()`   |
| `LoadSceneTimer.Install(duration, loader)`| `LoadSceneTimer.StartTimerWithDuration(duration)` |

`SceneLoader` is not a `MonoBehaviour` anymore and `MonoSceneLoaderDestroyer` was deleted, both were
created at runtime by the installer, so no scene has to be updated.

The `SceneLoaderInstaller` exposes `Time Before Unloading Transition Scene`, the seconds between
`OnTransitionSceneStartUnloaded` and the unload of the loading screen, which the 1.x loader waited
hardcoded. It defaults to 0.5 seconds, the same time that 1.x waited, so raise it if the loading
screen needs more time to fade out.

A scene that registers its `ICommandQueue` in the loader has to do it while it is loading, on `Awake`
or on `Start`, which is what `SceneLoadedNotifierInstaller` does. A scene that registers it later
does not stop the loading anymore, its commands run with the next scene.

## Tests

The tests run in the Unity Test Runner, `Tests/Editor` for the loading state machine, the commands
and the scene data, and `Tests/PlayMode` for the installer and the injector initializer.
