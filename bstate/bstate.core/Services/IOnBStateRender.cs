using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnBStateRender
{
    Task OnBStateRender(BStateComponent component);
}