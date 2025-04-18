using bstate.core.Classes;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

class PreProcessorRunnerMiddleware(IServiceProvider serviceProvider, BStateConfiguration bStateConfiguration) : IAsyncMiddleware<IAction>
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var preprocessors = bStateConfiguration.MiddlewareRegister.GetPreprocessors();
        
        foreach (var preprocessor in preprocessors)
        {
            var preProcessor = (IPreProcessor)serviceProvider.GetService(preprocessor);
            await preProcessor!.Run(parameter);
        }
        await next(parameter);
    }
}