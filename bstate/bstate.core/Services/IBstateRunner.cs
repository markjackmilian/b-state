using bstate.core.Middlewares;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

public interface IBstateRunner
{
    Task Run(IAction action);
}

class BstateRunner(IServiceProvider serviceProvider) : IBstateRunner
{
    public async Task Run(IAction action)
    {
        var pipeline = new AsyncPipeline<IAction>(new ServiceProviderMiddlewareResolver(serviceProvider))
            .Add<ActionRunnerMiddleware>()
            .Add<PostProcessorRenderer>();
        
        await pipeline.Execute(action);
    }
}