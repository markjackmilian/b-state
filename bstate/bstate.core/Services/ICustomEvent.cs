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
public interface IOnDisposeAsync
{
    Task OnDisposeAsync(BStateComponent component);
}

public interface IOnParametersSet
{
    void OnParametersSet(BStateComponent component);
}
