using bstate.core;
using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Components.Test;

public class TestPreprocessorMiddleware : IPreprocessorMiddleware
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        Console.WriteLine("TestPreprocessorMiddleware");
        return next(parameter);
    }
}

public class TestPostprocessorMiddleware : IPostProcessorMiddleware
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        Console.WriteLine("TestPostprocessorMiddleware");
        return next(parameter);
    }
}