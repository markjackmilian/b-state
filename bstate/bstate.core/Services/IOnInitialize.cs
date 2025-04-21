using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnInitialize
{
    Task OnInitialize(BStateComponent component);
}