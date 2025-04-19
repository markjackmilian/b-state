using System.Collections.Concurrent;
using bstate.core.Services;

namespace bstate.core.Components;

public partial class BStateComponent
{
    private readonly ConcurrentBag<Type> _onInitializes = new();
    
    protected void UseOnInitiaze<T>() where T : IOnInitialize
    {
        _onInitializes.Add(typeof(T));
    }
}