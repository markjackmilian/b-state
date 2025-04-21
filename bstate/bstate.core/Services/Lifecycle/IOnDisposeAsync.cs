using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnDisposeAsync : ILifeCycle
{
    Task OnDisposeAsync(BStateComponent component);
}