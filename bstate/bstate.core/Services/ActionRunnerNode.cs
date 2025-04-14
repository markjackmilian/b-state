using PipelineNet.Middleware;

namespace bstate.core.Services;

public class ActionRunnerNode(IServiceProvider serviceProvider) : IAsyncMiddleware<IAction>
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var parameterType = parameter.GetType();
        var handlerType = typeof(IActionHandler<>).MakeGenericType(parameterType);
        
        var handler = serviceProvider.GetService(handlerType);
        if (handler != null)
        {
            var executeMethod = handlerType.GetMethod("Execute");
            return (Task)executeMethod.Invoke(handler, [parameter]);
        }
    
        return next(parameter);
    }
}