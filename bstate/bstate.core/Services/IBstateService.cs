using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IBstateService
{
    T UseState<T>(ComponentBase component) where T : BState;
}

class BstateService(IComponentRegister componentRegister, IServiceProvider serviceProvider) : IBstateService
{
    public T UseState<T>(ComponentBase component) where T : BState
    {
        componentRegister.Add<T>(component);
        var state = serviceProvider.GetService<T>()!;
        return state;
    }
}