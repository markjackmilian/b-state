using bstate.core.Classes;
using bstate.core.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

public interface IActionBus
{
    Task Send(IAction action);
}

class ActionBus(IServiceProvider serviceProvider, IBStateConfiguration configuration) : IActionBus
{
    public async Task Send(IAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var builder = serviceProvider.GetService<IPipelineBuilder>();

        if (builder is null)
            throw new InvalidOperationException("Pipeline builder not found");
        
        var pipeline = builder
            .AddBeaviours(configuration.GetBehaviours())
            .AddPreprocessors()
            .AddActionRunner()
            .AddPostprocessors()
            .AddRenderer()
            .Build();

        await pipeline.Execute(action);
    }
}

