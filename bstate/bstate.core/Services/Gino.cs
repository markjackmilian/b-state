using PipelineNet.Middleware;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

public class Gino(IServiceProvider serviceProvider)
{
    public void Prova()
    {
        var pipeline = new AsyncPipeline<IAction>(new ServiceProviderMiddlewareResolver(serviceProvider))
            .Add<RoudCornersMiddleware>();

    }
}

public class RoudCornersMiddleware : IAsyncMiddleware<IAction>
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        throw new NotImplementedException();
    }
}


public class Unit{}