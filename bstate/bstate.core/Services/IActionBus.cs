using bstate.core.Classes;
using bstate.core.Middlewares;
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
        var pipeline = new PipelineBuilder(serviceProvider)
            .AddBeaviours(configuration.BehaviourRegister.GetBehaviours())
            .AddPreprocessors()
            .AddActionRunner()
            .AddPostprocessors()
            .AddRenderer()
            .Build();

        await pipeline.Execute(action);
    }
}

class PipelineBuilder(IServiceProvider serviceProvider)
{
    private readonly AsyncPipeline<IAction> _pipeline = new(new ServiceProviderMiddlewareResolver(serviceProvider));

    public PipelineBuilder AddPreprocessors()
    {
        _pipeline.Add<PreProcessorRunnerMiddleware>();
        return this;
    }

    public PipelineBuilder AddActionRunner()
    {
        _pipeline.Add<ActionRunnerMiddleware>();
        return this;
    }

    public PipelineBuilder AddPostprocessors()
    {
        _pipeline.Add<PostProcessorRunnerMiddleware>();
        return this;
    }

    public PipelineBuilder AddRenderer()
    {
        _pipeline.Add<PostProcessorRenderer>();
        return this;
    }
    
    public PipelineBuilder AddBeaviours(IEnumerable<Type> behaviours)
    {
        foreach (var behaviour in behaviours)
        {
            _pipeline.Add(behaviour);       
        }
        return this;
    }

    public AsyncPipeline<IAction> Build()
    {
        return _pipeline;
    }
}