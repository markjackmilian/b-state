using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IStore
{
    T Get<T>() where T : BState;
}

class Store(IServiceProvider serviceProvider) : IStore
{
    public T Get<T>() where T : BState => serviceProvider.GetService<T>();
    
}