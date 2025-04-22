using bstate.core.Classes;
using bstate.core.Middlewares;
using bstate.web.example.Classes;

namespace bstate.web.example.Pipeline;

class ALongActionPreProcessor<TRequest> : IPreProcessor<TRequest>
where TRequest : ILongAction
{
    public Task Run(IAction parameter)
    {
        Console.WriteLine("This is a long action... please wait!");
        return Task.CompletedTask;
    }
}