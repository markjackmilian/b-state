using bstate.core.Classes;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IBus
{
    Task Send(IAction action);
}

class Bus(IServiceProvider serviceProvider) : IBus
{
    public async Task Send(IAction action)
    {
        var pipeline = new PipelineBuilder(serviceProvider)
            .AddPreprocessors()
            .AddActionRunner()
            .AddPostprocessors()
            .AddRenderer()
            .Build();

        await pipeline.Execute(action);
    }
}