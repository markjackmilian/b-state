using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Pipeline;

class AllRequestPreprocessor<TAction> : IPreProcessor<TAction>
where TAction : IAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("AllRequestPreprocessor");
        return Task.CompletedTask;
    }
}

