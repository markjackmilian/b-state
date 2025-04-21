using bstate.core.Classes;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

class PostProcessorRunnerMiddleware(IServiceProvider serviceProvider) : IAsyncMiddleware<IAction>
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var genericType = typeof(IPostProcessorGeneric<>).MakeGenericType(parameter.GetType());
        var preProcessors = (IAbstractProcessor[])serviceProvider.GetServices(genericType);
        
        foreach (var preprocessor in preProcessors)
        {
            await preprocessor!.Run(parameter);
        }
        await next(parameter);
    }
}