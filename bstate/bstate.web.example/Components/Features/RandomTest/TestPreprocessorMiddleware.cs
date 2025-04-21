using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Components.Features.RandomTest;


public class TestPreprocessorGeneric : IPreProcessorGeneric<IAction>
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