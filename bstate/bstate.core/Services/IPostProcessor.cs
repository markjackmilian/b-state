using PipelineNet.Middleware;

namespace bstate.core.Services;

public interface IPostProcessor : IAsyncMiddleware<IAction>
{ }

class PostProcessorRenderer(IBStateRegister register) : IPostProcessor
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var components = register.GetComponents();
        foreach (var bStateComponent in components)
        {
            bStateComponent.BStateRender();
        }
        return Task.CompletedTask;
    }
}