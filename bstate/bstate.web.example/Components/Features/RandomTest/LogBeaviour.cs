using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Components.Features.RandomTest;

class LogBeaviour : IBehaviour
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        Console.WriteLine($"Action {parameter.GetType().Name} started");
        await next(parameter);
        Console.WriteLine($"Action {parameter.GetType().Name} completed");
    }
}