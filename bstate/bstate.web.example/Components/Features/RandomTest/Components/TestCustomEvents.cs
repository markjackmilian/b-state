using bstate.core.Services;

namespace bstate.web.example.Components.Features.RandomTest.Components;

public class TestCustomEvents : IOnInitialize
{
    public Task OnInitialize()
    {
        Console.WriteLine("TestCustomEvents oninitialize");
        return Task.CompletedTask;
    }
}