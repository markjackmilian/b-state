using PipelineNet.Middleware;

namespace bstate.core.Services;

public interface IPostProcessor : IAsyncMiddleware<IAction>
{ }

class PostProcessorRenderer(IComponentRegister register) : IPostProcessor
{
    public Task Run(IAction parameter, Func<IAction, Task> next)
    {
        var stateType = parameter.GetType().DeclaringType;
        
        var components = register.GetComponents(stateType);
        foreach (var bStateComponent in components)
        {
            bStateComponent.BStateRender();
        }
        return Task.CompletedTask;
    }
}