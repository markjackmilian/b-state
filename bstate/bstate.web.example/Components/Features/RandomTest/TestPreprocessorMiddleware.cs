using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Components.Features.RandomTest;


public class TestPreprocessorGeneric<T> : IPreProcessorGeneric<T>
where T : IAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("TestPreprocessorGenericMiddleware");
        return Task.CompletedTask;
    }
}

public class TestPreprocessorOnlyLongGeneric<TAction> : IPreProcessorGeneric<TAction> 
    where TAction : IsLongAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("TestPreprocessorOnlyLongGeneric for long action");
        return Task.CompletedTask;
    }
}


public class TestPostprocessorMiddleware<T> : IPostProcessorGeneric<T>
where T : IAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("TestPostprocessorMiddleware");
        return Task.CompletedTask;
    }
}