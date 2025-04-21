using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnDisposeAsync
{
    Task OnDisposeAsync(BStateComponent component);
}