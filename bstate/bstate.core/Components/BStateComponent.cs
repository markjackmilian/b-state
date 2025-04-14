using Microsoft.AspNetCore.Components;

namespace bstate.core.Components;

public abstract class BStateComponent : ComponentBase
{
    protected void UseState<T>() where T : BState
    {
           
    }
}