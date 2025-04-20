using System.Collections.Concurrent;
using bstate.core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Components;

public abstract partial class BStateComponent : ComponentBase, IAsyncDisposable
{
    [Inject]
    private IComponentRegister ComponentRegister { get; set; } 
    
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } 

    protected T UseState<T>() where T : BState
    {
        ComponentRegister.Add<T>(this);
        return ServiceProvider.GetService<T>()!;
    }
}