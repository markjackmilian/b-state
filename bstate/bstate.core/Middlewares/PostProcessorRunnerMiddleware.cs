using bstate.core.Classes;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

class PostProcessorRunnerMiddleware(IServiceProvider serviceProvider, BStateConfiguration bStateConfiguration) : IAsyncMiddleware<IAction>
{
    public async Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var preprocessors = bStateConfiguration.MiddlewareRegister.GetPostprocessors();
        
        foreach (var preprocessor in preprocessors)
        {
            var preProcessor = (IPostProcessor)serviceProvider.GetService(preprocessor);
            await preProcessor!.Run(parameter);
        }
        await next(parameter);
    }
}