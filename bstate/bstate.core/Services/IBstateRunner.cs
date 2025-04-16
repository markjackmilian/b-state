using bstate.core.Classes;
using bstate.core.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

public interface IBstateRunner
{
    Task Run(IAction action);
}

class BstateRunner(IServiceProvider serviceProvider, BStateConfiguration configuration) : IBstateRunner
{
    public async Task Run(IAction action)
    {
        var preprocessors = configuration.MiddlewareRegister.GetPreprocessors();
        
        var pipeline = new AsyncPipeline<IAction>(new ServiceProviderMiddlewareResolver(serviceProvider));

        foreach (var preprocessor in preprocessors)
        {
            pipeline.Add(preprocessor);
        }

        pipeline.Add<ActionRunnerMiddleware>();
        
        var postProcessors = configuration.MiddlewareRegister.GetPostprocessors();

        foreach (var postProcessor in postProcessors)
        {
            pipeline.Add(postProcessor);
        }
        
        pipeline.Add<PostProcessorRenderer>();

        await pipeline.Execute(action);
    }
}