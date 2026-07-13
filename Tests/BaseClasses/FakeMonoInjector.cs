using DependencyInjector.Installers;

namespace ScenesLoaderSystem.Tests
{
    public class FakeMonoInjector : BaseMonoInjector
    {
        private int _totalInjections;

        public int TotalInjections => _totalInjections;

        public override void InjectAll()
        {
            _totalInjections++;
        }

        public override void Dispose()
        {
        }
    }
}
