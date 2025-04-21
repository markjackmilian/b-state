using bstate.core.Classes;
using bstate.core.Middlewares;

namespace bstate.web.example.Features;

class LogBehaviour : IBehaviour
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        Console.WriteLine($"{parameter.GetType().Name} started");
        await next(parameter);
        Console.WriteLine($"{parameter.GetType().Name} ended");
    }
}