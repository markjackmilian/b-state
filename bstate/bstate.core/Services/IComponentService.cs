using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IComponentService
{
    T UseState<T>(ComponentBase component) where T : BState;
}

class ComponentService(IComponentRegister componentRegister, IServiceProvider serviceProvider) : IComponentService
{
    public T UseState<T>(ComponentBase component) where T : BState
    {
        componentRegister.Add<T>(component);
        var state = serviceProvider.GetService<T>()!;
        return state;
    }
}