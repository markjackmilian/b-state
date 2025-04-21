using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnDisposeAsync
{
    Task OnDisposeAsync(BStateComponent component);
}