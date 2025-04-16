using bstate.core.Classes;
using bstate.core.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

public interface IActionChannel
{
    Task Send(IAction action);
}

class ActionChannel(IServiceProvider serviceProvider, BStateConfiguration configuration) : IActionChannel
{
    public async Task Send(IAction action)
    {
        var pipeline = new PipelineBuilder(serviceProvider)
            .AddPreprocessors(configuration.MiddlewareRegister.GetPreprocessors())
            .AddActionRunner()
            .AddPostprocessors(configuration.MiddlewareRegister.GetPostprocessors())
            .AddRenderer()
            .Build();

        await pipeline.Execute(action);
    }
}

class PipelineBuilder(IServiceProvider serviceProvider)
{
    private readonly AsyncPipeline<IAction> _pipeline = new(new ServiceProviderMiddlewareResolver(serviceProvider));

    public PipelineBuilder AddPreprocessors(IEnumerable<Type> preprocessors)
    {
        foreach (var preprocessor in preprocessors)
        {
            _pipeline.Add(preprocessor);
        }
        return this;
    }

    public PipelineBuilder AddActionRunner()
    {
        _pipeline.Add<ActionRunnerMiddleware>();
        return this;
    }

    public PipelineBuilder AddPostprocessors(IEnumerable<Type> postProcessors)
    {
        foreach (var postProcessor in postProcessors)
        {
            _pipeline.Add(postProcessor);
        }
        return this;
    }

    public PipelineBuilder AddRenderer()
    {
        _pipeline.Add<PostProcessorRenderer>();
        return this;
    }

    public AsyncPipeline<IAction> Build()
    {
        return _pipeline;
    }
}