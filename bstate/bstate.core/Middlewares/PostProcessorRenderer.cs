using bstate.core.Classes;
using bstate.core.Services;
using PipelineNet.Middleware;

namespace bstate.core.Middlewares;

class PostProcessorRenderer(IComponentRegister register) : IAsyncMiddleware<IAction>
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