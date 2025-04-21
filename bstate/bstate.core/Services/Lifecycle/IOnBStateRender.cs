using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnBStateRender
{
    Task OnBStateRender(BStateComponent component);
}