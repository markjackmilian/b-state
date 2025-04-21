using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnInitialize
{
    Task OnInitialize(BStateComponent component);
}