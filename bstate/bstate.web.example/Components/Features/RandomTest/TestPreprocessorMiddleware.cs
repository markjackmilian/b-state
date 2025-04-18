using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Components.Features.RandomTest;

public class TestPreprocessorMiddleware : IPreProcessor
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("TestPreprocessorMiddleware");
        return Task.CompletedTask;
    }
}

public class TestPostprocessorMiddleware : IPostProcessor
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("TestPostprocessorMiddleware");
        return Task.CompletedTask;
    }
}