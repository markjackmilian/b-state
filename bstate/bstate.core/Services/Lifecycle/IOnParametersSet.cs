using bstate.core.Components;

namespace bstate.core.Services.Lifecycle;

public interface IOnParametersSet
{
    void OnParametersSet(BStateComponent component);
}