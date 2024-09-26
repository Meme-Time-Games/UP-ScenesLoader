namespace ScenesLoaderSystem
{
    public interface ISceneDataLoader
    {
        void Load(bool dontRemoveOpenScenes = false);
    }
}