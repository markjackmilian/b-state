using System.Diagnostics;
using PipelineNet.Middleware;

namespace bstate.core.Services;

public class ActionRunnerNode(IServiceProvider serviceProvider) : IAsyncMiddleware<IAction>
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var parameterType = parameter.GetType();
        var handlerType = typeof(IActionHandler<>).MakeGenericType(parameterType);
        
        var handler = serviceProvider.GetService(handlerType);
        if (handler != null)
        {
            var executeMethod = handlerType.GetMethod("Execute");
            var task = (Task)executeMethod!.Invoke(handler, [parameter]);
            Debug.Assert(task != null, nameof(task) + " != null");
            await task;
        }
    
        await next(parameter);
    }
}