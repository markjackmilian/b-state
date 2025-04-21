using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnAfterRenderAsync : ILifeCycle
{
    Task OnAfterRenderAsync(BStateComponent component, bool firstRender);
}