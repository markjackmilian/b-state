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

class ActionBus(IServiceProvider serviceProvider, BStateConfiguration configuration) : IActionBus
{
    public async Task Send(IAction action)
    {
        var builder = serviceProvider.GetService<IPipelineBuilder>();
        var pipeline = builder
            .AddBeaviours(configuration.BehaviourRegister.GetBehaviours())
            .AddPreprocessors()
            .AddActionRunner()
            .AddPostprocessors()
            .AddRenderer()
            .Build();

        await pipeline.Execute(action);
    }
}

