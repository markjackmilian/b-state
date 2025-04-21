using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnInitialize : ILifeCycle
{
    Task OnInitialize(BStateComponent component);
}