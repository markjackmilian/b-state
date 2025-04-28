using System.Collections.Concurrent;
using bstate.core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Components;

public abstract partial class BStateComponent : ComponentBase, IAsyncDisposable
{
    [Inject]
    internal IComponentRegister ComponentRegister { get; set; } 
    
    [Inject]
    internal IServiceProvider ServiceProvider { get; set; } 

    protected T UseState<T>() where T : BState
    {
        ComponentRegister.Add<T>(this);
        var state = ServiceProvider.GetService<T>()!;
        return state;
    }
}