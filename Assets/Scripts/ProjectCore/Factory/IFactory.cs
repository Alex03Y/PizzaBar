using ProjectCore.ServiceLocator;

namespace ProjectCore.Factory
{
    public interface IFactory : IService
    {
        
    }

    public interface IFactory<out TReturn, in TArgs> : IFactory where TArgs : IFactoryArgs
    {
        TReturn Create(TArgs args);
    }

    public interface IFactoryArgs
    {
    }
}