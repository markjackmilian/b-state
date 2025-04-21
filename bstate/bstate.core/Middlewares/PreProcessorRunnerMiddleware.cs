using bstate.core.Classes;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Middleware;
using System;

namespace bstate.core.Middlewares;

class PreProcessorRunnerMiddleware(IServiceProvider serviceProvider, BStateConfiguration bStateConfiguration) : IAsyncMiddleware<IAction>
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        
        var genericType = typeof(IPreProcessorGeneric<>).MakeGenericType(parameter.GetType());
        var preProcessors = (IAbstractProcessor[])serviceProvider.GetServices(genericType);
        // var preprocessors = bStateConfiguration.MiddlewareRegister.GetPreprocessors();
        
        foreach (var preprocessor in preProcessors)
        {
            
            //var preProcessor = (IAbstractProcessor)serviceProvider.GetService(genericType);
            await preprocessor!.Run(parameter);
        }
        await next(parameter);
    }
}