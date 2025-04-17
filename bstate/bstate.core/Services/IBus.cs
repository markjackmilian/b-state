using bstate.core.Classes;
using Microsoft.Extensions.DependencyInjection;

namespace bstate.core.Services;

public interface IBus
{
    Task Send(IAction action);
    Task Send<T>(IRequest<T> action);
}

class Bus(IServiceProvider serviceProvider, BStateConfiguration configuration) : IBus
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

    public Task Send<T>(IRequest<T> action)
    {
        throw new NotImplementedException();
    }
}