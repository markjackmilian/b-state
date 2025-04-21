using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnAfterRenderAsync
{
    Task OnAfterRenderAsync(BStateComponent component, bool firstRender);
}