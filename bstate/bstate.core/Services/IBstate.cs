using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IBstate
{
    T UseState<T>(ComponentBase component) where T : BState;
    void OnComponentDisposed(ComponentBase componentBase);
}

class Bstate(IComponentRegister componentRegister, IServiceProvider serviceProvider) : IBstate
{
    public T UseState<T>(ComponentBase component) where T : BState
    {
        componentRegister.Add<T>(component);
        var state = serviceProvider.GetService<T>()!;
        return state;
    }

    public void OnComponentDisposed(ComponentBase componentBase)
    {
        componentRegister.Clear(componentBase);
    }
}