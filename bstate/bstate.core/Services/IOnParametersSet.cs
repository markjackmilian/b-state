using bstate.core.Components;

namespace bstate.core.Services;

public interface IOnParametersSet
{
    void OnParametersSet(BStateComponent component);
}