
namespace KoikatuVR.Interpreters
{
    abstract class SceneInterpreter
    {
        public abstract void OnStart();
        public abstract void OnDisable();
        public abstract void OnUpdate();
    }
}
