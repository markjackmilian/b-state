using bstate.core.Classes;
using bstate.core.Middlewares;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Services;

internal interface IPipelineBuilder
{
    IPipelineBuilder AddPreprocessors();
    IPipelineBuilder AddActionRunner();
    IPipelineBuilder AddPostprocessors();
    IPipelineBuilder AddRenderer();
    IPipelineBuilder AddBehaviours(IEnumerable<Type> behaviours);
    IAsyncPipeline<IAction> Build();
}

internal class PipelineBuilder(IServiceProvider serviceProvider) : IPipelineBuilder
{
    private readonly AsyncPipeline<IAction> _pipeline = new(new ServiceProviderMiddlewareResolver(serviceProvider));

    public IPipelineBuilder AddPreprocessors()
    {
        _pipeline.Add<PreProcessorRunnerMiddleware>();
        return this;
    }

    public IPipelineBuilder AddActionRunner()
    {
        _pipeline.Add<ActionRunnerMiddleware>();
        return this;
    }

    public IPipelineBuilder AddPostprocessors()
    {
        _pipeline.Add<PostProcessorRunnerMiddleware>();
        return this;
    }

    public IPipelineBuilder AddRenderer()
    {
        _pipeline.Add<PostProcessorRenderer>();
        return this;
    }
    
    public IPipelineBuilder AddBehaviours(IEnumerable<Type> behaviours)
    {
        foreach (var behaviour in behaviours)
        {
            _pipeline.Add(behaviour);       
        }
        return this;
    }

    public IAsyncPipeline<IAction> Build()
    {
        return _pipeline;
    }
}