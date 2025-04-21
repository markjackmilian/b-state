using bstate.core.Components;
using bstate.core.Services.Lifecycle;

namespace bstate.web.example.Features.Counter;

/// <summary>
///  This is an example to know when a component receive a render command from BState
/// </summary>
class CounterOnRender : IOnBStateRender
{
    public Task OnBStateRender(BStateComponent component)
    { 
        Console.WriteLine("Counter OnRender");
        return Task.CompletedTask;
    }
}