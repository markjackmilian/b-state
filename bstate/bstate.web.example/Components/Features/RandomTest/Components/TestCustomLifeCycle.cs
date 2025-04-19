using bstate.core.Components;
using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest.Components;

public class TestCustomLifeCycle : IOnInitialize, IOnAfterRenderAsync
{
    public Task OnInitialize(BStateComponent component)
    {
        Console.WriteLine($"{component.GetType().Name} - TestCustomEvents oninitialize");
        return Task.CompletedTask;
    }

    public Task OnAfterRenderAsync(BStateComponent component, bool firstRender)
    {
        Console.WriteLine($"{component.GetType().Name} - TestCustomEvents onafterrenderasync - firstRender: {firstRender}");
        return Task.CompletedTask;
    }
}