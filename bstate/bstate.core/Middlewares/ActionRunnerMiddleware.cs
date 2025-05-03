using bstate.core.Classes;
using PipelineNet.Middleware;
using System.Collections.Concurrent;
using System.Reflection;

namespace bstate.core.Middlewares;

public class ActionRunnerMiddleware(IServiceProvider serviceProvider) : IAsyncMiddleware<IAction>
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> ExecuteMethodCache = new();

    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var parameterType = parameter.GetType();

        var handlerType = HandlerTypeCache.GetOrAdd(parameterType, type =>
            typeof(IActionHandler<>).MakeGenericType(type));

        var handler = serviceProvider.GetService(handlerType);

        if (handler != null)
        {
            var executeMethod = ExecuteMethodCache.GetOrAdd(handlerType, type =>
                type.GetMethod("Execute"));

            var task = (Task)executeMethod!.Invoke(handler, [parameter]);
            await task!;
        }

        await next(parameter);
    }
}