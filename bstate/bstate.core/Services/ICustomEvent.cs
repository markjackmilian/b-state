using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnInitialize
{
    Task OnInitialize(BStateComponent component);
}

public interface IOnAfterRenderAsync
{
    Task OnAfterRenderAsync(BStateComponent component, bool firstRender);
}
public interface IOnBStateRender
{
    Task OnBStateRender(BStateComponent component);
}