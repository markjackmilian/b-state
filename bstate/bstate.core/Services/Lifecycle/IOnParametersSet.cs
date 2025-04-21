using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnParametersSet : ILifeCycle
{
    void OnParametersSet(BStateComponent component);
}