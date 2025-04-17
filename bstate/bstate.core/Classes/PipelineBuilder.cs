using bstate.core.Middlewares;
using PipelineNet.Pipelines;
using PipelineNet.ServiceProvider.MiddlewareResolver;

namespace bstate.core.Classes;

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