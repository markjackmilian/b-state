using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnBStateRender : ILifeCycle
{
    Task OnBStateRender(BStateComponent component);
}